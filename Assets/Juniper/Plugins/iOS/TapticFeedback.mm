/// <summary>
/// The feedback pattern to play when a new selection is made.
/// </summary>
static UISelectionFeedbackGenerator *selection;

/// <summary>
/// The feedback pattern to play when a light hit is made.
/// </summary>
static UIImpactFeedbackGenerator *light;

/// <summary>
/// The feedback pattern to play when a medium hit is made.
/// </summary>
static UIImpactFeedbackGenerator *medium;

/// <summary>
/// The feedback pattern to play when a heavy hit is made.
/// </summary>
static UIImpactFeedbackGenerator *heavy;

/// <summary>
/// The feedback pattern to play when a notification occurs.
/// </summary>
static UINotificationFeedbackGenerator *notification;

extern "C"
{
    /// <summary>
    /// Creates the reusable vibration pattern objects and prepares
    /// them for firing.
    /// </summary>
    void Prepare()
    {
        selection = [[UISelectionFeedbackGenerator alloc] init];
        light = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
        medium = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
        heavy = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
        notification = [[UINotificationFeedbackGenerator alloc] init];

        [selection prepare];
        [light prepare];
        [medium prepare];
        [heavy prepare];
        [notification prepare];
    }

    /// <summary>
    /// Play one of the Taptic feedback patterns:
    ///   * 1 -> selection changed
    ///   * 2 -> light impact
    ///   * 3 -> medium impact
    ///   * 4 -> heavy impact
    ///   * 5 -> warning notification
    ///   * 6 -> error notification
    ///   * 7 -> success notification.
    /// </summary>
    void PlayTaptic(unsigned int index)
    {
        if(index == 1)
        {
            [selection selectionChanged];
        }
        else if(index == 2)
        {
            [light impactOccurred];
        }
        else if(index == 3)
        {
            [medium impactOccurred];
        }
        else if(index == 4)
        {
            [heavy impactOccurred];
        }
        else if(index == 5)
        {
            [notification notificationOccurred:UINotificationFeedbackTypeWarning];
        }
        else if(index == 6)
        {
            [notification notificationOccurred:UINotificationFeedbackTypeError];
        }
        else if(index == 7)
        {
            [notification notificationOccurred:UINotificationFeedbackTypeSuccess];
        }
        else
        {
            printf("TapticFeedback: Attempted to pass an invalid taptic index to PlayTaptic");
        }
    }

    /// <summary>
    /// Nil-s out the <see cref="selection"/>, <see cref="light"/>,
    /// <see cref="medium"/>, <see cref="heavy"/>, and
    /// <see cref="notification"/> objects so they can get picked
    /// up by the garbage collector.
    /// </summary>
    void Shutdown()
    {
        selection = nil;
        light = nil;
        medium = nil;
        heavy = nil;
        notification = nil;
    }
}
