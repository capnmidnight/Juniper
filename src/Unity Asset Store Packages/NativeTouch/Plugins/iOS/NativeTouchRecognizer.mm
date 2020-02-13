//#define LOG_TOUCH
//#define LOG_UNITY_MESSAGE

//With this on you will gain no new data at all, but each callback can contains overlapping data with other callbacks.
//e.g. If moved and ended callback invoked at the same time each one might have the same A and B data. (A being moved, B being ended)
//With multitouch you will get a Stationary phase for other finger staying still at that moment.
//But at the same time it is also possible to infer stationary phase without this. And anyways you cannot get
//Stationary with single touch.
//#define ALL_TOUCHES

#import "NativeTouchRecognizer.h"
#import "UnityAppController.h"
#import "UnityView.h"
#import "UnityInterface.h"

@implementation NativeTouchRecognizer

//Singleton instance variable
NativeTouchRecognizer* gestureRecognizer;

UnityView* unityView;
CGRect screenSize;
CGFloat screenScale;

struct NativeTouchData
{
    int callbackType;
    float x;
    float y;
    float previousX;
    float previousY;
    int phase;
    double timestamp;
    int pointerId; //not available but has to be there to align with Android
    int nativelyGenerated;
};

struct NativeTouchDataFull
{
    int callbackType;
    float x;
    float y;
    float previousX;
    float previousY;
    int phase;
    double timestamp;
    int pointerId; //not available but has to be there to align with Android
    int nativelyGenerated;
    
    //-- Full mode only structs --
    
    int tapCount;
    int type;
    float force;
    float maximumPossibleForce;
    float majorRadius;
    float majorRadiusTolerance;
    float altitudeAngle;
    float azimuthAngleInView;
};

//This type of delegate tells C# to check the ring buffer starting from `start` index and goes by `count`.
//That's all the new touches.
typedef void (*NativeTouchCheckRingBufferDelegate)(int start, int count);

int ringBufferSize = -1;
int ringBufferIndex = 0;
int ringBufferCurrentCount = 0;

NativeTouchCheckRingBufferDelegate fullCallbackCheckRingBuffer;
NativeTouchCheckRingBufferDelegate minimalCallbackCheckRingBuffer;

//Receive ring buffer space from C#, write on native side.
NativeTouchData* ntdRingBuffer;
NativeTouchDataFull* ntdFullRingBuffer;

int* finalCursor; //Set from C#, here we just focus on increasing it.
int* dekker; //int[3] at C#

bool isFullMode;
bool isDisableUnityTouch;
bool isNoCallback;

bool isStopped;

+ (int) RealScreenWidth
{
    UnityAppController* uiApp = GetAppController();
    return [[uiApp unityView] bounds].size.width *[[UIScreen mainScreen] nativeScale];
}

+ (int) RealScreenHeight
{
    UnityAppController* uiApp = GetAppController();
    return [[uiApp unityView] bounds].size.height * [[UIScreen mainScreen] nativeScale];
}

+ (void) StopNativeTouch
{
    //Hack : On detaching gesture recognizer Unity would somehow "replay"
    //all the touches that would have gone to the view normally 2 frames after the remove.
    //This could cause problem if those touch replays are pressing things.
    
    //Those replays comes through normal iOS entry point of touches like
    //-(void) touchesBegan:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
    //So it even looks like iOS's bug rather than Unity's bug.
    
    //For example, pressing Stop button in the demo after playing around a bit
    //would result in the touch replay replays everything and finally the final touch at the Stop
    //button. This would always cause you to Stop twice on clicking the button.

    //If you play around too much before removing the recognizer,
    //you would even get error called "out of free touches!" on replay. (how descriptive)
    
    //Solution 1 : We do not ever detech gesture recognizer. Stop native touch keep the recognizer but
    //turn off the path to Native Touch leaving just forwarding touches to Unity.
    
    //Cons of this approach is that if other plugins wants to use recognizers too, it may cause problem
    //when you have 2 active recognizers on Unity view..
    
    //Solution 2 : We remove gesture recognizer,
    //but use UnitySetViewTouchProcessing(unityView, touchesIgnored); to prevent touch replay
    //until 2 frames later then use UnitySetViewTouchProcessing again to restore.
    //this 2 frames wait would have to be via Unity coroutine, and we would have to create
    //a dummy GameObject to host it.
    
    //We will go with Solution 1.
    isStopped = true;
    
    //[unityView removeGestureRecognizer:gestureRecognizer];
    //UnitySetViewTouchProcessing(unityView, touchesIgnored);
}

+ (void) StopNativeTouchFinalize
{
}

+ (void) StartNativeTouch
{
    UnityAppController* uiApp = GetAppController();
    
    isStopped = false;
    
    unityView = [uiApp unityView];
    screenScale = [[UIScreen mainScreen] nativeScale];
    screenSize = [unityView bounds];
    
#ifdef LOG_UNITY_MESSAGE
    NSLog(@"Starting native touch - \n Unity Bounds : %@ \n Screen Bounds : %@ \n Screen Native Bounds : %@ \n Scale : %f Native Scale : %f",
          NSStringFromCGRect([unityView bounds]),
          NSStringFromCGRect([[UIScreen mainScreen] bounds]),
          NSStringFromCGRect([[UIScreen mainScreen] nativeBounds]),
          [[UIScreen mainScreen] scale],
          [[UIScreen mainScreen] nativeScale]
      );
#endif
    
    if(gestureRecognizer == nil)
    {
        gestureRecognizer = [[NativeTouchRecognizer alloc] init];
        [unityView addGestureRecognizer:gestureRecognizer];
    }
}

+ (double) GetNativeTouchTime
{
    return [[NSProcessInfo processInfo] systemUptime];
} 

+(CGPoint) scaledCGPoint:(CGPoint)point
{
    //Retina display have 2x scale and have a smallest unit of pixel as 0.5.
    //This will multiply it back and eliminate the floating point
    
    //Also "plus" devices have 3x scale and now each coordinate could be in either
    //.00
    //.33332824707031
    //.66665649414062
    //Unfortunately when it is not a perfect integer, multiplying with 3
    //gives something like .99996948242188 which sucks, since we could no longer rely on
    //casting to `int` to get the correct whole number pixel.
    
    //I could round it, but I guess casting to int would be faster, so I will just add some small number
    //so that it would be floored by casting down.. it should be fine.
    
    //0,0 is at the top left of your orientation, max WxH is at bottom right.
    //We will flip this manually in Unity and not here, there is some information not available here
    //like "dynamic scaling" that Unity didn't expose that scaled size to native.
    
    //This is an inefficient but correct way.
    //return CGPointMake(round(point.x*screenScale), round(point.y*screenScale));
    
    //This is hackish way
    return CGPointMake((point.x*screenScale) + 0.001f, (point.y*screenScale) + 0.001f);
}

#ifdef LOG_TOUCH
+(void) logTouches:(NSSet<UITouch*> *) touches
{
    for(UITouch* touch in touches) {
        NSLog(@"Loc:%@ -> %@ \n Prev:%@ -> %@ \n Radius:%f \n Phase:%ld",
              NSStringFromCGPoint([touch locationInView:nil]),
              NSStringFromCGPoint([NativeTouchRecognizer scaledCGPoint:[touch locationInView:nil]]),
              NSStringFromCGPoint([touch previousLocationInView:nil]),
              NSStringFromCGPoint([NativeTouchRecognizer scaledCGPoint:[touch previousLocationInView:nil]]),
              [touch majorRadius],
              (long)[touch phase]);
    }
}
#endif

//This is a utility method for logging
+(const char*) encodeTouch: (UITouch*) touch
{
    CGPoint location = [NativeTouchRecognizer scaledCGPoint:[touch locationInView:nil]];
    CGPoint plocation = [NativeTouchRecognizer scaledCGPoint:[touch previousLocationInView:nil]];
    return [[NSString stringWithFormat:@"XY [%d, %d] PrevXY [%d, %d] %@",
             (int)location.x,
             (int)location.y,
             (int)plocation.x,
             (int)plocation.y,
             [NSString stringWithCString:[NativeTouchRecognizer phaseToString:[touch phase]] encoding:NSUTF8StringEncoding]
             ]UTF8String];
    //Phase : Began Moved Stationary Ended Cancelled : 0 1 2 3 4
}

+(const char*) phaseToString: (int) phase
{
    switch (phase) {
        case 0: return "Began";
            case 1: return "Moved";
            case 2: return "Stationary";
            case 3: return "Ended";
            case 4: return "Cancelled";
        default: return "???";
    }
}

+(void) sendTouchesToUnity:(NSSet<UITouch*> *) touches callbackType:(int) type
{
    if(isStopped) return;
    
    [NativeTouchRecognizer startTouches];
#ifdef LOG_UNITY_MESSAGE
    NSLog(@"-- Callback type : %@ --", [NSString stringWithCString:[NativeTouchRecognizer phaseToString:type]]);
#endif
    for(UITouch* touch in touches)
    {
#ifdef LOG_UNITY_MESSAGE
        NSLog(@"%@",[NSString stringWithCString:[NativeTouchRecognizer encodeTouch:touch] encoding:NSUTF8StringEncoding]);
#endif
        
        CGPoint location = [NativeTouchRecognizer scaledCGPoint:[touch locationInView:nil]];
        CGPoint previousLocation = [NativeTouchRecognizer scaledCGPoint:[touch previousLocationInView:nil]];
        
        if(isFullMode)
        {
            NativeTouchDataFull* ntd = &ntdFullRingBuffer[(ringBufferIndex + ringBufferCurrentCount) % ringBufferSize];

            ntd->callbackType = type;
            ntd->x = (float)location.x;
            ntd->y = (float)location.y;
            ntd->previousX = (int) previousLocation.x;
            ntd->previousY = (int) previousLocation.y;
            ntd->phase = (int)[touch phase];
            ntd->timestamp = [touch timestamp];
            ntd->nativelyGenerated = 1; //At C# side this has no setter, the only way it could be 1 is by here.
            
            ntd->tapCount = (int) [touch tapCount];
            ntd->type = (int)[touch type];
            ntd->force = (float)[touch force];
            ntd->maximumPossibleForce = (float)[touch maximumPossibleForce];
            ntd->majorRadius = (float)[touch majorRadius];
            ntd->majorRadiusTolerance = (float)[touch majorRadiusTolerance];
            ntd->altitudeAngle = (float)[touch altitudeAngle];
            ntd->azimuthAngleInView = (float)[touch azimuthAngleInView:nil];

            ringBufferCurrentCount++;

            (*finalCursor)++;
        }
        else
        {
            NativeTouchData* ntd = &ntdRingBuffer[(ringBufferIndex + ringBufferCurrentCount) % ringBufferSize];

            ntd->callbackType = type;
            ntd->x = (float)location.x;
            ntd->y = (float)location.y;
            ntd->previousX = (int)previousLocation.x;
            ntd->previousY = (int) previousLocation.y;
            ntd->phase = (int)[touch phase];
            ntd->timestamp = [touch timestamp];
            ntd->nativelyGenerated = 1; //At C# side this has no setter, the only way it could be 1 is by here.

            ringBufferCurrentCount++;

            (*finalCursor)++;
        }
    }
    
    if(isNoCallback == false)
    {
        if(isFullMode)
        {
            [NativeTouchRecognizer commitTouchesFull];
        }
        else
        {
            [NativeTouchRecognizer commitTouchesMinimal];
        }
    }
}

+(void) startTouches
{
    ringBufferIndex = (ringBufferIndex + ringBufferCurrentCount) % ringBufferSize;
    ringBufferCurrentCount = 0;
}

+(void) commitTouchesMinimal
{
    minimalCallbackCheckRingBuffer(ringBufferIndex, ringBufferCurrentCount);
}

+(void) commitTouchesFull
{
    fullCallbackCheckRingBuffer(ringBufferIndex, ringBufferCurrentCount);
}

-(void) touchesBegan:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
#ifdef LOG_TOUCH
    [NativeTouchRecognizer logTouches:touches];
#endif
    
#ifdef ALL_TOUCHES
    [NativeTouchRecognizer sendTouchesToUnity:[event allTouches] callbackType:0];
#else
    [NativeTouchRecognizer sendTouchesToUnity:touches callbackType:0];
#endif
    
    if(!isDisableUnityTouch || isStopped)
    {
        UnitySendTouchesBegin(touches, event);
    }
}

-(void) touchesEnded:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
#ifdef LOG_TOUCH
    [NativeTouchRecognizer logTouches:touches];
#endif
    
#ifdef ALL_TOUCHES
    [NativeTouchRecognizer sendTouchesToUnity:[event allTouches] callbackType:3];
#else
    [NativeTouchRecognizer sendTouchesToUnity:touches callbackType:3];
#endif
    
    if(!isDisableUnityTouch || isStopped)
    {
        UnitySendTouchesEnded(touches, event);
    }
}

-(void) touchesMoved:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
#ifdef LOG_TOUCH
    [NativeTouchRecognizer logTouches:touches];
#endif
    
#ifdef ALL_TOUCHES
    [NativeTouchRecognizer sendTouchesToUnity:[event allTouches] callbackType:1];
#else
    [NativeTouchRecognizer sendTouchesToUnity:touches callbackType:1];
#endif
    
    if(!isDisableUnityTouch || isStopped)
    {
        UnitySendTouchesMoved(touches, event);
    }
}

-(void) touchesCancelled:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
#ifdef LOG_TOUCH
    [NativeTouchRecognizer logTouches:touches];
#endif
    
#ifdef ALL_TOUCHES
    [NativeTouchRecognizer sendTouchesToUnity:[event allTouches] callbackType:4];
#else
    [NativeTouchRecognizer sendTouchesToUnity:touches callbackType:4];
#endif
    
    if(!isDisableUnityTouch || isStopped)
    {
        UnitySendTouchesCancelled(touches, event);
    }
}

@end

extern "C" {
    
    void _StopNativeTouch() {
        [NativeTouchRecognizer StopNativeTouch];
    }

    void _StartNativeTouch(
        int fullMode, 
        int disableUnityTouch,
        int noCallback,
        NativeTouchCheckRingBufferDelegate fullDelegate,
        NativeTouchCheckRingBufferDelegate minimalDelegate,
        NativeTouchDataFull* fullRingBuffer,
        NativeTouchData* minimalRingBuffer,
        int* finalCursorHandle,
        int* dekkerHandle, //We don't need this in iOS actually since its not threaded, but so that the code can be copy pasted from Android
        int ringBufferSizeFromCSharp
    )
    {
        isDisableUnityTouch = disableUnityTouch == 1 ? true : false;
        isFullMode = fullMode == 1 ? true : false;
        isNoCallback = noCallback == 1 ? true : false;

        ringBufferSize = ringBufferSizeFromCSharp;
        ringBufferIndex = 0;
        ringBufferCurrentCount = 0;

        fullCallbackCheckRingBuffer = fullDelegate;
        minimalCallbackCheckRingBuffer = minimalDelegate;

        ntdFullRingBuffer = fullRingBuffer;
        ntdRingBuffer = minimalRingBuffer;

        finalCursor = finalCursorHandle;
        dekker = dekkerHandle;
        
        [NativeTouchRecognizer StartNativeTouch];
    }

    
    double _GetNativeTouchTime() {
        return [NativeTouchRecognizer GetNativeTouchTime];
    }
    
    int _RealScreenWidth() {
        return [NativeTouchRecognizer RealScreenWidth];
    }
    
    int _RealScreenHeight() {
        return [NativeTouchRecognizer RealScreenHeight];
    }
    
}
