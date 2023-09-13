using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class Emoji
    {
        // This one has to be first
        public static readonly Emoji zeroWidthJoiner = new("\u200D", "Zero Width Joiner");

        public static readonly Emoji textStyle = new("\uFE0E", "Variation Selector-15: text style");
        public static readonly Emoji emojiStyle = new("\uFE0F", "Variation Selector-16: emoji style");

        public static readonly Emoji combiningEnclosingKeycap = new("\u20E3", "Combining Enclosing Keycap");
        public static readonly IReadOnlyList<Emoji> combiners = new[] {
            textStyle,
            emojiStyle,
            zeroWidthJoiner,
            combiningEnclosingKeycap,
        };

        public static readonly Emoji female = new("\u2640" + emojiStyle, "Female");
        public static readonly Emoji male = new("\u2642" + emojiStyle, "Male");
        public static readonly Emoji transgender = new("\u26A7" + emojiStyle, "Transgender Symbol");
        public static readonly IReadOnlyList<Emoji> sexes = new[] {
            female,
            male,
        };
        public static readonly Emoji skinL = new(0x1F3FB, "Light Skin Tone");
        public static readonly Emoji skinML = new(0x1F3FC, "Medium-Light Skin Tone");
        public static readonly Emoji skinM = new(0x1F3FD, "Medium Skin Tone");
        public static readonly Emoji skinMD = new(0x1F3FE, "Medium-Dark Skin Tone");
        public static readonly Emoji skinD = new(0x1F3FF, "Dark Skin Tone");
        public static readonly IReadOnlyList<Emoji> skinTones = new[] {
            skinL,
            skinML,
            skinM,
            skinMD,
            skinD,
        };
        public static readonly Emoji hairRed = new(0x1F9B0, "Red Hair");
        public static readonly Emoji hairCurly = new(0x1F9B1, "Curly Hair");
        public static readonly Emoji hairWhite = new(0x1F9B3, "White Hair");
        public static readonly Emoji hairBald = new(0x1F9B2, "Bald");
        public static readonly IReadOnlyList<Emoji> hairColors = new[] {
            hairRed,
            hairCurly,
            hairWhite,
            hairBald,
        };

        private static Emoji Combo(string description, params Emoji[] parts)
        {
            var partValues = parts.Select(p => p.Value);
            var value = string.Join("", partValues);
            return new(value, description);
        }

        private static EmojiGroup Join(EmojiGroup A, Emoji b)
        {
            var temp = A.Join(b);
            var alts = A.Alts.Select(a => a.Join(b)).ToArray();
            return new(temp, alts);
        }

        private static EmojiGroup Skin(int v, string d, params Emoji[] rest) =>
            Skin(char.ConvertFromUtf32(v), d, rest);

        private static EmojiGroup Skin(string v, string d, params Emoji[] rest)
        {
            var person = new Emoji(v, d);
            var alts = new[]
            {
                person,
                person + skinL,
                person + skinML,
                person + skinM,
                person + skinMD,
                person + skinD
            }.Concat(rest)
            .ToArray();
            return new(v, d, alts);
        }

        private static EmojiGroup Sex(Emoji person) =>
            new(person,
                person,
                person.Join(male),
                person.Join(female));

        private static EmojiGroup SkinAndSex(int v, string d) =>
            SkinAndSex(char.ConvertFromUtf32(v), d);

        private static EmojiGroup SkinAndSex(string v, string d) =>
            Sex(Skin(v, d));

        private static EmojiGroup SkinAndHair(int v, string d, params Emoji[] rest) =>
            SkinAndHair(char.ConvertFromUtf32(v), d, rest);

        private static EmojiGroup SkinAndHair(string v, string d, params Emoji[] rest)
        {
            var person = Skin(v, d);
            var red = Join(person, hairRed);
            var curly = Join(person, hairCurly);
            var white = Join(person, hairWhite);
            var bald = Join(person, hairBald);
            var alts = new[]
            {
                person,
                red,
                curly,
                white,
                bald
            }.Concat(rest)
            .ToArray();
            return new(v, d, alts);
        }

        private static EmojiGroup Symbol(Emoji symbol, string name)
        {
            var j = new Emoji(symbol, name);
            var men = man.Alts[0].Join(j);
            var women = woman.Alts[0].Join(j);
            return new(symbol, symbol, men, women);
        }

        public static readonly EmojiGroup frowners = SkinAndSex(0x1F64D, "Frowning");
        public static readonly EmojiGroup pouters = SkinAndSex(0x1F64E, "Pouting");
        public static readonly EmojiGroup gesturingNo = SkinAndSex(0x1F645, "Gesturing NO");
        public static readonly EmojiGroup gesturingOK = SkinAndSex(0x1F646, "Gesturing OK");
        public static readonly EmojiGroup tippingHand = SkinAndSex(0x1F481, "Tipping Hand");
        public static readonly EmojiGroup raisingHand = SkinAndSex(0x1F64B, "Raising Hand");
        public static readonly EmojiGroup bowing = SkinAndSex(0x1F647, "Bowing");
        public static readonly EmojiGroup facePalming = SkinAndSex(0x1F926, "Facepalming");
        public static readonly EmojiGroup shrugging = SkinAndSex(0x1F937, "Shrugging");
        public static readonly EmojiGroup cantHear = SkinAndSex(0x1F9CF, "Can't Hear");
        public static readonly EmojiGroup gettingMassage = SkinAndSex(0x1F486, "Getting Massage");
        public static readonly EmojiGroup gettingHaircut = SkinAndSex(0x1F487, "Getting Haircut");

        public static readonly EmojiGroup constructionWorkers = SkinAndSex(0x1F477, "Construction Worker");
        public static readonly EmojiGroup guards = SkinAndSex(0x1F482, "Guard");
        public static readonly EmojiGroup spies = SkinAndSex(0x1F575, "Spy");
        public static readonly EmojiGroup police = SkinAndSex(0x1F46E, "Police");
        public static readonly EmojiGroup wearingTurban = SkinAndSex(0x1F473, "Wearing Turban");
        public static readonly EmojiGroup superheroes = SkinAndSex(0x1F9B8, "Superhero");
        public static readonly EmojiGroup supervillains = SkinAndSex(0x1F9B9, "Supervillain");
        public static readonly EmojiGroup mages = SkinAndSex(0x1F9D9, "Mage");
        public static readonly EmojiGroup fairies = SkinAndSex(0x1F9DA, "Fairy");
        public static readonly EmojiGroup vampires = SkinAndSex(0x1F9DB, "Vampire");
        public static readonly EmojiGroup merpeople = SkinAndSex(0x1F9DC, "Merperson");
        public static readonly EmojiGroup elves = SkinAndSex(0x1F9DD, "Elf");
        public static readonly EmojiGroup walking = SkinAndSex(0x1F6B6, "Walking");
        public static readonly EmojiGroup standing = SkinAndSex(0x1F9CD, "Standing");
        public static readonly EmojiGroup kneeling = SkinAndSex(0x1F9CE, "Kneeling");
        public static readonly EmojiGroup runners = SkinAndSex(0x1F3C3, "Running");

        public static readonly EmojiGroup gestures = new(
            "Gestures", "Gestures",
            frowners,
            pouters,
            gesturingNo,
            gesturingOK,
            tippingHand,
            raisingHand,
            bowing,
            facePalming,
            shrugging,
            cantHear,
            gettingMassage,
            gettingHaircut);


        public static readonly EmojiGroup baby = Skin(0x1F476, "Baby");
        public static readonly EmojiGroup child = Skin(0x1F9D2, "Child");
        public static readonly EmojiGroup boy = Skin(0x1F466, "Boy");
        public static readonly EmojiGroup girl = Skin(0x1F467, "Girl");
        public static readonly EmojiGroup children = new(child, child, boy, girl);


        public static readonly EmojiGroup blondes = SkinAndSex(0x1F471, "Blond Person");
        public static readonly EmojiGroup person = Skin(0x1F9D1, "Person",
            blondes,
            wearingTurban);

        public static readonly EmojiGroup beardedMan = Skin(0x1F9D4, "Bearded Man");
        public static readonly Emoji manInSuitLevitating = new(0x1F574 + emojiStyle, "Man in Suit, Levitating");
        public static readonly EmojiGroup manWithChineseCap = Skin(0x1F472, "Man With Chinese Cap");
        public static readonly EmojiGroup manInTuxedo = Skin(0x1F935, "Man in Tuxedo");
        public static readonly EmojiGroup man = SkinAndHair(0x1F468, "Man",
            blondes.Alts[1],
            beardedMan,
            manInSuitLevitating,
            manWithChineseCap,
            wearingTurban.Alts[0],
            manInTuxedo);

        public static readonly EmojiGroup pregnantWoman = Skin(0x1F930, "Pregnant Woman");
        public static readonly EmojiGroup breastFeeding = Skin(0x1F931, "Breast-Feeding");
        public static readonly EmojiGroup womanWithHeadscarf = Skin(0x1F9D5, "Woman With Headscarf");
        public static readonly EmojiGroup brideWithVeil = Skin(0x1F470, "Bride With Veil");
        public static readonly EmojiGroup woman = SkinAndHair(0x1F469, "Woman",
            blondes.Alts[2],
            pregnantWoman,
            breastFeeding,
            womanWithHeadscarf,
            wearingTurban.Alts[1],
            brideWithVeil);

        public static readonly EmojiGroup adults = new(person.Value, "Adult", person, man, woman);

        public static readonly EmojiGroup olderPerson = Skin(0x1F9D3, "Older Person");
        public static readonly EmojiGroup oldMan = Skin(0x1F474, "Old Man");
        public static readonly EmojiGroup oldWoman = Skin(0x1F475, "Old Woman");
        public static readonly EmojiGroup elderly = new(olderPerson, olderPerson, oldMan, oldWoman);

        public static readonly Emoji medical = new("\u2695" + emojiStyle, "Medical");
        public static readonly EmojiGroup healthCareWorkers = Symbol(medical, "Health Care");

        public static readonly Emoji graduationCap = new(0x1F393, "Graduation Cap");
        public static readonly EmojiGroup students = Symbol(graduationCap, "Student");

        public static readonly Emoji school = new(0x1F3EB, "School");
        public static readonly EmojiGroup teachers = Symbol(school, "Teacher");

        public static readonly Emoji balanceScale = new("\u2696" + emojiStyle, "Balance Scale");
        public static readonly EmojiGroup judges = Symbol(balanceScale, "Judge");

        public static readonly Emoji sheafOfRice = new(0x1F33E, "Sheaf of Rice");
        public static readonly EmojiGroup farmers = Symbol(sheafOfRice, "Farmer");

        public static readonly Emoji cooking = new(0x1F373, "Cooking");
        public static readonly EmojiGroup cooks = Symbol(cooking, "Cook");

        public static readonly Emoji wrench = new(0x1F527, "Wrench");
        public static readonly EmojiGroup mechanics = Symbol(wrench, "Mechanic");

        public static readonly Emoji factory = new(0x1F3ED, "Factory");
        public static readonly EmojiGroup factoryWorkers = Symbol(factory, "Factory Worker");

        public static readonly Emoji briefcase = new(0x1F4BC, "Briefcase");
        public static readonly EmojiGroup officeWorkers = Symbol(briefcase, "Office Worker");

        public static readonly Emoji fireEngine = new(0x1F692, "Fire Engine");
        public static readonly EmojiGroup fireFighters = Symbol(fireEngine, "Fire Fighter");

        public static readonly Emoji rocket = new(0x1F680, "Rocket");
        public static readonly EmojiGroup astronauts = Symbol(rocket, "Astronaut");

        public static readonly Emoji airplane = new("\u2708" + emojiStyle, "Airplane");
        public static readonly EmojiGroup pilots = Symbol(airplane, "Pilot");

        public static readonly Emoji artistPalette = new(0x1F3A8, "Artist Palette");
        public static readonly EmojiGroup artists = Symbol(artistPalette, "Artist");

        public static readonly Emoji microphone = new(0x1F3A4, "Microphone");
        public static readonly EmojiGroup singers = Symbol(microphone, "Singer");

        public static readonly Emoji laptop = new(0x1F4BB, "Laptop");
        public static readonly EmojiGroup technologists = Symbol(laptop, "Technologist");

        public static readonly Emoji microscope = new(0x1F52C, "Microscope");
        public static readonly EmojiGroup scientists = Symbol(microscope, "Scientist");

        public static readonly Emoji crown = new(0x1F451, "Crown");
        public static readonly EmojiGroup prince = Skin(0x1F934, "Prince");
        public static readonly EmojiGroup princess = Skin(0x1F478, "Princess");
        public static readonly EmojiGroup royalty = new(crown, crown, prince, princess);

        public static readonly EmojiGroup roles = new(
            "Roles", "Depictions of people working",
            healthCareWorkers,
            students,
            teachers,
            judges,
            farmers,
            cooks,
            mechanics,
            factoryWorkers,
            officeWorkers,
            scientists,
            technologists,
            singers,
            artists,
            pilots,
            astronauts,
            fireFighters,
            spies,
            guards,
            constructionWorkers,
            royalty);

        public static readonly EmojiGroup cherub = Skin(0x1F47C, "Cherub");
        public static readonly EmojiGroup santaClaus = Skin(0x1F385, "Santa Claus");
        public static readonly EmojiGroup mrsClaus = Skin(0x1F936, "Mrs. Claus");

        public static readonly Emoji genie = new(0x1F9DE, "Genie");
        public static readonly EmojiGroup genies = Sex(genie);
        public static readonly Emoji zombie = new(0x1F9DF, "Zombie");
        public static readonly EmojiGroup zombies = Sex(zombie);

        public static readonly EmojiGroup fantasy = new(
            "Fantasy", "Depictions of fantasy characters",
            cherub,
            santaClaus,
            mrsClaus,
            superheroes,
            supervillains,
            mages,
            fairies,
            vampires,
            merpeople,
            elves,
            genies,
            zombies);

        public static readonly Emoji whiteCane = new(0x1F9AF, "Probing Cane");
        public static readonly EmojiGroup withProbingCane = Symbol(whiteCane, "Probing");

        public static readonly Emoji motorizedWheelchair = new(0x1F9BC, "Motorized Wheelchair");
        public static readonly EmojiGroup inMotorizedWheelchair = Symbol(motorizedWheelchair, "In Motorized Wheelchair");

        public static readonly Emoji manualWheelchair = new(0x1F9BD, "Manual Wheelchair");
        public static readonly EmojiGroup inManualWheelchair = Symbol(manualWheelchair, "In Manual Wheelchair");


        public static readonly EmojiGroup manDancing = Skin(0x1F57A, "Man Dancing");
        public static readonly EmojiGroup womanDancing = Skin(0x1F483, "Woman Dancing");
        public static readonly EmojiGroup dancers = new(manDancing.Value, "Dancing", manDancing, womanDancing);

        public static readonly EmojiGroup jugglers = SkinAndSex(0x1F939, "Juggler");

        public static readonly EmojiGroup climbers = SkinAndSex(0x1F9D7, "Climber");
        public static readonly Emoji fencer = new(0x1F93A, "Fencer");
        public static readonly EmojiGroup jockeys = Skin(0x1F3C7, "Jockey");
        public static readonly Emoji skier = new("\u26F7" + emojiStyle, "Skier");
        public static readonly EmojiGroup snowboarders = Skin(0x1F3C2, "Snowboarder");
        public static readonly EmojiGroup golfers = SkinAndSex(0x1F3CC + emojiStyle, "Golfer");
        public static readonly EmojiGroup surfers = SkinAndSex(0x1F3C4, "Surfing");
        public static readonly EmojiGroup rowers = SkinAndSex(0x1F6A3, "Rowing Boat");
        public static readonly EmojiGroup swimmers = SkinAndSex(0x1F3CA, "Swimming");
        public static readonly EmojiGroup basketballers = SkinAndSex("\u26F9" + emojiStyle, "Basket Baller");
        public static readonly EmojiGroup weightLifters = SkinAndSex(0x1F3CB + emojiStyle, "Weight Lifter");
        public static readonly EmojiGroup bikers = SkinAndSex(0x1F6B4, "Biker");
        public static readonly EmojiGroup mountainBikers = SkinAndSex(0x1F6B5, "Mountain Biker");
        public static readonly EmojiGroup cartwheelers = SkinAndSex(0x1F938, "Cartwheeler");
        public static readonly Emoji wrestler = new(0x1F93C, "Wrestler");
        public static readonly EmojiGroup wrestlers = Sex(wrestler);
        public static readonly EmojiGroup waterPoloers = SkinAndSex(0x1F93D, "Water Polo Player");
        public static readonly EmojiGroup handBallers = SkinAndSex(0x1F93E, "Hand Baller");

        public static readonly EmojiGroup inMotion = new(
            "In Motion", "Depictions of people in motion",
            walking,
            standing,
            kneeling,
            withProbingCane,
            inMotorizedWheelchair,
            inManualWheelchair,
            dancers,
            jugglers,
            climbers,
            fencer,
            jockeys,
            skier,
            snowboarders,
            golfers,
            surfers,
            rowers,
            swimmers,
            runners,
            basketballers,
            weightLifters,
            bikers,
            mountainBikers,
            cartwheelers,
            wrestlers,
            waterPoloers,
            handBallers);

        public static readonly EmojiGroup inLotusPosition = SkinAndSex(0x1F9D8, "In Lotus Position");
        public static readonly EmojiGroup inBath = Skin(0x1F6C0, "In Bath");
        public static readonly EmojiGroup inBed = Skin(0x1F6CC, "In Bed");
        public static readonly EmojiGroup inSauna = SkinAndSex(0x1F9D6, "In Sauna");
        public static readonly EmojiGroup resting = new(
            "Resting", "Depictions of people at rest",
            inLotusPosition,
            inBath,
            inBed,
            inSauna);

        public static readonly EmojiGroup babies = new(baby, baby, cherub);
        public static readonly EmojiGroup people = new(
            "People", "People",
            babies,
            children,
            adults,
            elderly);

        public static readonly EmojiGroup allPeople = new(
            "All People", "All People",
            people,
            gestures,
            inMotion,
            resting,
            roles,
            fantasy);

        public static readonly Emoji ogre = new(0x1F479, "Ogre");
        public static readonly Emoji goblin = new(0x1F47A, "Goblin");
        public static readonly Emoji ghost = new(0x1F47B, "Ghost");
        public static readonly Emoji alien = new(0x1F47D, "Alien");
        public static readonly Emoji alienMonster = new(0x1F47E, "Alien Monster");
        public static readonly Emoji angryFaceWithHorns = new(0x1F47F, "Angry Face with Horns");
        public static readonly Emoji skull = new(0x1F480, "Skull");
        public static readonly Emoji pileOfPoo = new(0x1F4A9, "Pile of Poo");
        public static readonly Emoji grinningFace = new(0x1F600, "Grinning Face");
        public static readonly Emoji beamingFaceWithSmilingEyes = new(0x1F601, "Beaming Face with Smiling Eyes");
        public static readonly Emoji faceWithTearsOfJoy = new(0x1F602, "Face with Tears of Joy");
        public static readonly Emoji grinningFaceWithBigEyes = new(0x1F603, "Grinning Face with Big Eyes");
        public static readonly Emoji grinningFaceWithSmilingEyes = new(0x1F604, "Grinning Face with Smiling Eyes");
        public static readonly Emoji grinningFaceWithSweat = new(0x1F605, "Grinning Face with Sweat");
        public static readonly Emoji grinningSquitingFace = new(0x1F606, "Grinning Squinting Face");
        public static readonly Emoji smillingFaceWithHalo = new(0x1F607, "Smiling Face with Halo");
        public static readonly Emoji smilingFaceWithHorns = new(0x1F608, "Smiling Face with Horns");
        public static readonly Emoji winkingFace = new(0x1F609, "Winking Face");
        public static readonly Emoji smilingFaceWithSmilingEyes = new(0x1F60A, "Smiling Face with Smiling Eyes");
        public static readonly Emoji faceSavoringFood = new(0x1F60B, "Face Savoring Food");
        public static readonly Emoji relievedFace = new(0x1F60C, "Relieved Face");
        public static readonly Emoji smilingFaceWithHeartEyes = new(0x1F60D, "Smiling Face with Heart-Eyes");
        public static readonly Emoji smilingFaceWithSunglasses = new(0x1F60E, "Smiling Face with Sunglasses");
        public static readonly Emoji smirkingFace = new(0x1F60F, "Smirking Face");
        public static readonly Emoji neutralFace = new(0x1F610, "Neutral Face");
        public static readonly Emoji expressionlessFace = new(0x1F611, "Expressionless Face");
        public static readonly Emoji unamusedFace = new(0x1F612, "Unamused Face");
        public static readonly Emoji downcastFaceWithSweat = new(0x1F613, "Downcast Face with Sweat");
        public static readonly Emoji pensiveFace = new(0x1F614, "Pensive Face");
        public static readonly Emoji confusedFace = new(0x1F615, "Confused Face");
        public static readonly Emoji confoundedFace = new(0x1F616, "Confounded Face");
        public static readonly Emoji kissingFace = new(0x1F617, "Kissing Face");
        public static readonly Emoji faceBlowingAKiss = new(0x1F618, "Face Blowing a Kiss");
        public static readonly Emoji kissingFaceWithSmilingEyes = new(0x1F619, "Kissing Face with Smiling Eyes");
        public static readonly Emoji kissingFaceWithClosedEyes = new(0x1F61A, "Kissing Face with Closed Eyes");
        public static readonly Emoji faceWithTongue = new(0x1F61B, "Face with Tongue");
        public static readonly Emoji winkingFaceWithTongue = new(0x1F61C, "Winking Face with Tongue");
        public static readonly Emoji squintingFaceWithTongue = new(0x1F61D, "Squinting Face with Tongue");
        public static readonly Emoji disappointedFace = new(0x1F61E, "Disappointed Face");
        public static readonly Emoji worriedFace = new(0x1F61F, "Worried Face");
        public static readonly Emoji angryFace = new(0x1F620, "Angry Face");
        public static readonly Emoji poutingFace = new(0x1F621, "Pouting Face");
        public static readonly Emoji cryingFace = new(0x1F622, "Crying Face");
        public static readonly Emoji perseveringFace = new(0x1F623, "Persevering Face");
        public static readonly Emoji faceWithSteamFromNose = new(0x1F624, "Face with Steam From Nose");
        public static readonly Emoji sadButRelievedFace = new(0x1F625, "Sad but Relieved Face");
        public static readonly Emoji frowningFaceWithOpenMouth = new(0x1F626, "Frowning Face with Open Mouth");
        public static readonly Emoji anguishedFace = new(0x1F627, "Anguished Face");
        public static readonly Emoji fearfulFace = new(0x1F628, "Fearful Face");
        public static readonly Emoji wearyFace = new(0x1F629, "Weary Face");
        public static readonly Emoji sleepyFace = new(0x1F62A, "Sleepy Face");
        public static readonly Emoji tiredFace = new(0x1F62B, "Tired Face");
        public static readonly Emoji grimacingFace = new(0x1F62C, "Grimacing Face");
        public static readonly Emoji loudlyCryingFace = new(0x1F62D, "Loudly Crying Face");
        public static readonly Emoji faceWithOpenMouth = new(0x1F62E, "Face with Open Mouth");
        public static readonly Emoji hushedFace = new(0x1F62F, "Hushed Face");
        public static readonly Emoji anxiousFaceWithSweat = new(0x1F630, "Anxious Face with Sweat");
        public static readonly Emoji faceScreamingInFear = new(0x1F631, "Face Screaming in Fear");
        public static readonly Emoji astonishedFace = new(0x1F632, "Astonished Face");
        public static readonly Emoji flushedFace = new(0x1F633, "Flushed Face");
        public static readonly Emoji sleepingFace = new(0x1F634, "Sleeping Face");
        public static readonly Emoji dizzyFace = new(0x1F635, "Dizzy Face");
        public static readonly Emoji faceWithoutMouth = new(0x1F636, "Face Without Mouth");
        public static readonly Emoji faceWithMedicalMask = new(0x1F637, "Face with Medical Mask");
        public static readonly Emoji grinningCatWithSmilingEyes = new(0x1F638, "Grinning Cat with Smiling Eyes");
        public static readonly Emoji catWithTearsOfJoy = new(0x1F639, "Cat with Tears of Joy");
        public static readonly Emoji grinningCat = new(0x1F63A, "Grinning Cat");
        public static readonly Emoji smilingCatWithHeartEyes = new(0x1F63B, "Smiling Cat with Heart-Eyes");
        public static readonly Emoji catWithWrySmile = new(0x1F63C, "Cat with Wry Smile");
        public static readonly Emoji kissingCat = new(0x1F63D, "Kissing Cat");
        public static readonly Emoji poutingCat = new(0x1F63E, "Pouting Cat");
        public static readonly Emoji cryingCat = new(0x1F63F, "Crying Cat");
        public static readonly Emoji wearyCat = new(0x1F640, "Weary Cat");
        public static readonly Emoji slightlyFrowningFace = new(0x1F641, "Slightly Frowning Face");
        public static readonly Emoji slightlySmilingFace = new(0x1F642, "Slightly Smiling Face");
        public static readonly Emoji updisdeDownFace = new(0x1F643, "Upside-Down Face");
        public static readonly Emoji faceWithRollingEyes = new(0x1F644, "Face with Rolling Eyes");
        public static readonly Emoji seeNoEvilMonkey = new(0x1F648, "See-No-Evil Monkey");
        public static readonly Emoji hearNoEvilMonkey = new(0x1F649, "Hear-No-Evil Monkey");
        public static readonly Emoji speakNoEvilMonkey = new(0x1F64A, "Speak-No-Evil Monkey");
        public static readonly Emoji zipperMouthFace = new(0x1F910, "Zipper-Mouth Face");
        public static readonly Emoji moneyMouthFace = new(0x1F911, "Money-Mouth Face");
        public static readonly Emoji faceWithThermometer = new(0x1F912, "Face with Thermometer");
        public static readonly Emoji nerdFace = new(0x1F913, "Nerd Face");
        public static readonly Emoji thinkingFace = new(0x1F914, "Thinking Face");
        public static readonly Emoji faceWithHeadBandage = new(0x1F915, "Face with Head-Bandage");
        public static readonly Emoji robot = new(0x1F916, "Robot");
        public static readonly Emoji huggingFace = new(0x1F917, "Hugging Face");
        public static readonly Emoji cowboyHatFace = new(0x1F920, "Cowboy Hat Face");
        public static readonly Emoji clownFace = new(0x1F921, "Clown Face");
        public static readonly Emoji nauseatedFace = new(0x1F922, "Nauseated Face");
        public static readonly Emoji rollingOnTheFloorLaughing = new(0x1F923, "Rolling on the Floor Laughing");
        public static readonly Emoji droolingFace = new(0x1F924, "Drooling Face");
        public static readonly Emoji lyingFace = new(0x1F925, "Lying Face");
        public static readonly Emoji sneezingFace = new(0x1F927, "Sneezing Face");
        public static readonly Emoji faceWithRaisedEyebrow = new(0x1F928, "Face with Raised Eyebrow");
        public static readonly Emoji starStruck = new(0x1F929, "Star-Struck");
        public static readonly Emoji zanyFace = new(0x1F92A, "Zany Face");
        public static readonly Emoji shushingFace = new(0x1F92B, "Shushing Face");
        public static readonly Emoji faceWithSymbolsOnMouth = new(0x1F92C, "Face with Symbols on Mouth");
        public static readonly Emoji faceWithHandOverMouth = new(0x1F92D, "Face with Hand Over Mouth");
        public static readonly Emoji faceVomitting = new(0x1F92E, "Face Vomiting");
        public static readonly Emoji explodingHead = new(0x1F92F, "Exploding Head");
        public static readonly Emoji smilingFaceWithHearts = new(0x1F970, "Smiling Face with Hearts");
        public static readonly Emoji yawningFace = new(0x1F971, "Yawning Face");
        public static readonly Emoji smilingFaceWithTear = new(0x1F972, "Smiling Face with Tear");
        public static readonly Emoji partyingFace = new(0x1F973, "Partying Face");
        public static readonly Emoji woozyFace = new(0x1F974, "Woozy Face");
        public static readonly Emoji hotFace = new(0x1F975, "Hot Face");
        public static readonly Emoji coldFace = new(0x1F976, "Cold Face");
        public static readonly Emoji disguisedFace = new(0x1F978, "Disguised Face");
        public static readonly Emoji pleadingFace = new(0x1F97A, "Pleading Face");
        public static readonly Emoji faceWithMonocle = new(0x1F9D0, "Face with Monocle");
        public static readonly Emoji skullAndCrossbones = new("\u2620" + emojiStyle, "Skull and Crossbones");
        public static readonly Emoji frowningFace = new("\u2639" + emojiStyle, "Frowning Face");
        public static readonly Emoji smilingFace = new("\u263A" + emojiStyle, "Smiling Face");
        public static readonly Emoji speakingHead = new(0x1F5E3 + emojiStyle, "Speaking Head");
        public static readonly Emoji bust = new(0x1F464, "Bust in Silhouette");
        public static readonly EmojiGroup faces = new(
            "Faces", "Round emoji faces",
            ogre,
            goblin,
            ghost,
            alien,
            alienMonster,
            angryFaceWithHorns,
            skull,
            pileOfPoo,
            grinningFace,
            beamingFaceWithSmilingEyes,
            faceWithTearsOfJoy,
            grinningFaceWithBigEyes,
            grinningFaceWithSmilingEyes,
            grinningFaceWithSweat,
            grinningSquitingFace,
            smillingFaceWithHalo,
            smilingFaceWithHorns,
            winkingFace,
            smilingFaceWithSmilingEyes,
            faceSavoringFood,
            relievedFace,
            smilingFaceWithHeartEyes,
            smilingFaceWithSunglasses,
            smirkingFace,
            neutralFace,
            expressionlessFace,
            unamusedFace,
            downcastFaceWithSweat,
            pensiveFace,
            confusedFace,
            confoundedFace,
            kissingFace,
            faceBlowingAKiss,
            kissingFaceWithSmilingEyes,
            kissingFaceWithClosedEyes,
            faceWithTongue,
            winkingFaceWithTongue,
            squintingFaceWithTongue,
            disappointedFace,
            worriedFace,
            angryFace,
            poutingFace,
            cryingFace,
            perseveringFace,
            faceWithSteamFromNose,
            sadButRelievedFace,
            frowningFaceWithOpenMouth,
            anguishedFace,
            fearfulFace,
            wearyFace,
            sleepyFace,
            tiredFace,
            grimacingFace,
            loudlyCryingFace,
            faceWithOpenMouth,
            hushedFace,
            anxiousFaceWithSweat,
            faceScreamingInFear,
            astonishedFace,
            flushedFace,
            sleepingFace,
            dizzyFace,
            faceWithoutMouth,
            faceWithMedicalMask,
            grinningCatWithSmilingEyes,
            catWithTearsOfJoy,
            grinningCat,
            smilingCatWithHeartEyes,
            catWithWrySmile,
            kissingCat,
            poutingCat,
            cryingCat,
            wearyCat,
            slightlyFrowningFace,
            slightlySmilingFace,
            updisdeDownFace,
            faceWithRollingEyes,
            seeNoEvilMonkey,
            hearNoEvilMonkey,
            speakNoEvilMonkey,
            zipperMouthFace,
            moneyMouthFace,
            faceWithThermometer,
            nerdFace,
            thinkingFace,
            faceWithHeadBandage,
            robot,
            huggingFace,
            cowboyHatFace,
            clownFace,
            nauseatedFace,
            rollingOnTheFloorLaughing,
            droolingFace,
            lyingFace,
            sneezingFace,
            faceWithRaisedEyebrow,
            starStruck,
            zanyFace,
            shushingFace,
            faceWithSymbolsOnMouth,
            faceWithHandOverMouth,
            faceVomitting,
            explodingHead,
            smilingFaceWithHearts,
            yawningFace,
            smilingFaceWithTear,
            partyingFace,
            woozyFace,
            hotFace,
            coldFace,
            disguisedFace,
            pleadingFace,
            faceWithMonocle,
            skullAndCrossbones,
            frowningFace,
            smilingFace,
            speakingHead,
            bust);

        public static readonly Emoji kissMark = new(0x1F48B, "Kiss Mark");
        public static readonly Emoji loveLetter = new(0x1F48C, "Love Letter");
        public static readonly Emoji beatingHeart = new(0x1F493, "Beating Heart");
        public static readonly Emoji brokenHeart = new(0x1F494, "Broken Heart");
        public static readonly Emoji twoHearts = new(0x1F495, "Two Hearts");
        public static readonly Emoji sparklingHeart = new(0x1F496, "Sparkling Heart");
        public static readonly Emoji growingHeart = new(0x1F497, "Growing Heart");
        public static readonly Emoji heartWithArrow = new(0x1F498, "Heart with Arrow");
        public static readonly Emoji blueHeart = new(0x1F499, "Blue Heart");
        public static readonly Emoji greenHeart = new(0x1F49A, "Green Heart");
        public static readonly Emoji yellowHeart = new(0x1F49B, "Yellow Heart");
        public static readonly Emoji purpleHeart = new(0x1F49C, "Purple Heart");
        public static readonly Emoji heartWithRibbon = new(0x1F49D, "Heart with Ribbon");
        public static readonly Emoji revolvingHearts = new(0x1F49E, "Revolving Hearts");
        public static readonly Emoji heartDecoration = new(0x1F49F, "Heart Decoration");
        public static readonly Emoji blackHeart = new(0x1F5A4, "Black Heart");
        public static readonly Emoji whiteHeart = new(0x1F90D, "White Heart");
        public static readonly Emoji brownHeart = new(0x1F90E, "Brown Heart");
        public static readonly Emoji orangeHeart = new(0x1F9E1, "Orange Heart");
        public static readonly Emoji heartExclamation = new("\u2763" + emojiStyle, "Heart Exclamation");
        public static readonly Emoji redHeart = new("\u2764" + emojiStyle, "Red Heart");
        public static readonly EmojiGroup love = new(
            "Love", "Hearts and kisses",
            kissMark,
            loveLetter,
            beatingHeart,
            brokenHeart,
            twoHearts,
            sparklingHeart,
            growingHeart,
            heartWithArrow,
            blueHeart,
            greenHeart,
            yellowHeart,
            purpleHeart,
            heartWithRibbon,
            revolvingHearts,
            heartDecoration,
            blackHeart,
            whiteHeart,
            brownHeart,
            orangeHeart,
            heartExclamation,
            redHeart);

        public static readonly Emoji angerSymbol = new(0x1F4A2, "Anger Symbol");
        public static readonly Emoji bomb = new(0x1F4A3, "Bomb");
        public static readonly Emoji zzz = new(0x1F4A4, "Zzz");
        public static readonly Emoji collision = new(0x1F4A5, "Collision");
        public static readonly Emoji sweatDroplets = new(0x1F4A6, "Sweat Droplets");
        public static readonly Emoji dashingAway = new(0x1F4A8, "Dashing Away");
        public static readonly Emoji dizzy = new(0x1F4AB, "Dizzy");
        public static readonly Emoji speechBalloon = new(0x1F4AC, "Speech Balloon");
        public static readonly Emoji thoughtBalloon = new(0x1F4AD, "Thought Balloon");
        public static readonly Emoji hundredPoints = new(0x1F4AF, "Hundred Points");
        public static readonly Emoji hole = new(0x1F573 + emojiStyle, "Hole");
        public static readonly Emoji leftSpeechBubble = new(0x1F5E8 + emojiStyle, "Left Speech Bubble");
        public static readonly Emoji rightSpeechBubble = new(0x1F5E9 + emojiStyle, "Right Speech Bubble");
        public static readonly Emoji conversationBubbles2 = new(0x1F5EA + emojiStyle, "Conversation Bubbles 2");
        public static readonly Emoji conversationBubbles3 = new(0x1F5EB + emojiStyle, "Conversation Bubbles 3");
        public static readonly Emoji leftThoughtBubble = new(0x1F5EC + emojiStyle, "Left Thought Bubble");
        public static readonly Emoji rightThoughtBubble = new(0x1F5ED + emojiStyle, "Right Thought Bubble");
        public static readonly Emoji leftAngerBubble = new(0x1F5EE + emojiStyle, "Left Anger Bubble");
        public static readonly Emoji rightAngerBubble = new(0x1F5EF + emojiStyle, "Right Anger Bubble");
        public static readonly Emoji angerBubble = new(0x1F5F0 + emojiStyle, "Anger Bubble");
        public static readonly Emoji angerBubbleLightningBolt = new(0x1F5F1 + emojiStyle, "Anger Bubble Lightning");
        public static readonly Emoji lightningBolt = new(0x1F5F2 + emojiStyle, "Lightning Bolt");

        public static readonly EmojiGroup cartoon = new(
            "Cartoon", "Cartoon symbols",
            angerSymbol,
            bomb,
            zzz,
            collision,
            sweatDroplets,
            dashingAway,
            dizzy,
            speechBalloon,
            thoughtBalloon,
            hundredPoints,
            hole,
            leftSpeechBubble,
            rightSpeechBubble,
            conversationBubbles2,
            conversationBubbles3,
            leftThoughtBubble,
            rightThoughtBubble,
            leftAngerBubble,
            rightAngerBubble,
            angerBubble,
            angerBubbleLightningBolt,
            lightningBolt);

        public static readonly Emoji backhandIndexPointingUp = new(0x1F446, "Backhand Index Pointing Up");
        public static readonly Emoji backhandIndexPointingDown = new(0x1F447, "Backhand Index Pointing Down");
        public static readonly Emoji backhandIndexPointingLeft = new(0x1F448, "Backhand Index Pointing Left");
        public static readonly Emoji backhandIndexPointingRight = new(0x1F449, "Backhand Index Pointing Right");
        public static readonly Emoji oncomingFist = new(0x1F44A, "Oncoming Fist");
        public static readonly Emoji wavingHand = new(0x1F44B, "Waving Hand");
        public static readonly Emoji okHand = new(0x1F58F, "OK Hand");
        public static readonly Emoji thumbsUp = new(0x1F44D, "Thumbs Up");
        public static readonly Emoji thumbsDown = new(0x1F44E, "Thumbs Down");
        public static readonly Emoji clappingHands = new(0x1F44F, "Clapping Hands");
        public static readonly Emoji openHands = new(0x1F450, "Open Hands");
        public static readonly Emoji nailPolish = new(0x1F485, "Nail Polish");
        public static readonly Emoji handsWithFingersSplayed = new(0x1F590 + emojiStyle, "Hand with Fingers Splayed");
        public static readonly Emoji handsWithFingersSplayed2 = new(0x1F591 + emojiStyle, "Hand with Fingers Splayed 2");
        public static readonly Emoji thumbsUp2 = new(0x1F592, "Thumbs Up 2");
        public static readonly Emoji thumbsDown2 = new(0x1F593, "Thumbs Down 2");
        public static readonly Emoji peaceFingers = new(0x1F594, "Peace Fingers");
        public static readonly Emoji middleFinger = new(0x1F595, "Middle Finger");
        public static readonly Emoji vulcanSalute = new(0x1F596, "Vulcan Salute");
        public static readonly Emoji handPointingDown = new(0x1F597, "Hand Pointing Down");
        public static readonly Emoji handPointingLeft = new(0x1F598, "Hand Pointing Left");
        public static readonly Emoji handPointingRight = new(0x1F599, "Hand Pointing Right");
        public static readonly Emoji handPointingLeft2 = new(0x1F59A, "Hand Pointing Left 2");
        public static readonly Emoji handPointingRight2 = new(0x1F59B, "Hand Pointing Right 2");
        public static readonly Emoji indexPointingLeft = new(0x1F59C, "Index Pointing Left");
        public static readonly Emoji indexPointingRight = new(0x1F59D, "Index Pointing Right");
        public static readonly Emoji indexPointingUp = new(0x1F59E, "Index Pointing Up");
        public static readonly Emoji indexPointingDown = new(0x1F59F, "Index Pointing Down");
        public static readonly Emoji indexPointingUp2 = new(0x1F5A0, "Index Pointing Up 2");
        public static readonly Emoji indexPointingDown2 = new(0x1F5A1, "Index Pointing Down 2");
        public static readonly Emoji indexPointingUp3 = new(0x1F5A2, "Index Pointing Up 3");
        public static readonly Emoji indexPointingDown3 = new(0x1F5A3, "Index Pointing Down 3");
        public static readonly Emoji raisingHands = new(0x1F64C, "Raising Hands");
        public static readonly Emoji foldedHands = new(0x1F64F, "Folded Hands");
        public static readonly Emoji pinchedFingers = new(0x1F90C, "Pinched Fingers");
        public static readonly Emoji pinchingHand = new(0x1F90F, "Pinching Hand");
        public static readonly Emoji signOfTheHorns = new(0x1F918, "Sign of the Horns");
        public static readonly Emoji callMeHand = new(0x1F919, "Call Me Hand");
        public static readonly Emoji rasiedBackOfHand = new(0x1F91A, "Raised Back of Hand");
        public static readonly Emoji leftFacingFist = new(0x1F91B, "Left-Facing Fist");
        public static readonly Emoji rightFacingFist = new(0x1F91C, "Right-Facing Fist");
        public static readonly Emoji handshake = new(0x1F91D, "Handshake");
        public static readonly Emoji crossedFingers = new(0x1F91E, "Crossed Fingers");
        public static readonly Emoji loveYouGesture = new(0x1F91F, "Love-You Gesture");
        public static readonly Emoji palmsUpTogether = new(0x1F932, "Palms Up Together");
        public static readonly Emoji indexPointingUp4 = new("\u261D" + emojiStyle, "Index Pointing Up 4");
        public static readonly Emoji raisedFist = new("\u270A", "Raised Fist");
        public static readonly Emoji raisedHand = new("\u270B", "Raised Hand");
        public static readonly Emoji victoryHand = new("\u270C" + emojiStyle, "Victory Hand");
        public static readonly Emoji writingHand = new("\u270D" + emojiStyle, "Writing Hand");
        public static readonly EmojiGroup hands = new(
            "Hands", "Hands pointing at things",
            backhandIndexPointingUp,
            backhandIndexPointingDown,
            backhandIndexPointingLeft,
            backhandIndexPointingRight,
            oncomingFist,
            wavingHand,
            okHand,
            thumbsUp,
            thumbsDown,
            clappingHands,
            openHands,
            nailPolish,
            handsWithFingersSplayed,
            handsWithFingersSplayed2,
            handsWithFingersSplayed2,
            thumbsUp2,
            thumbsDown2,
            peaceFingers,
            middleFinger,
            vulcanSalute,
            handPointingDown,
            handPointingLeft,
            handPointingRight,
            handPointingLeft2,
            handPointingRight2,
            indexPointingLeft,
            indexPointingRight,
            indexPointingUp,
            indexPointingDown,
            indexPointingUp2,
            indexPointingDown2,
            indexPointingUp3,
            indexPointingDown3,
            raisingHands,
            foldedHands,
            pinchedFingers,
            pinchingHand,
            signOfTheHorns,
            callMeHand,
            rasiedBackOfHand,
            leftFacingFist,
            rightFacingFist,
            handshake,
            crossedFingers,
            loveYouGesture,
            palmsUpTogether,
            indexPointingUp4,
            raisedFist,
            raisedHand,
            victoryHand,
            writingHand);

        public static readonly EmojiGroup bodyParts = new(
            "Body Parts", "General body parts",
            new(0x1F440, "Eyes"),
            new(0x1F441 + emojiStyle, "Eye"),
            new(0x1F441 + emojiStyle + zeroWidthJoiner + 0x1F5E8 + emojiStyle, "Eye in Speech Bubble"),
            new(0x1F442, "Ear"),
            new(0x1F443, "Nose"),
            new(0x1F444, "Mouth"),
            new(0x1F445, "Tongue"),
            new(0x1F4AA, "Flexed Biceps"),
            new(0x1F933, "Selfie"),
            new(0x1F9B4, "Bone"),
            new(0x1F9B5, "Leg"),
            new(0x1F9B6, "Foot"),
            new(0x1F9B7, "Tooth"),
            new(0x1F9BB, "Ear with Hearing Aid"),
            new(0x1F9BE, "Mechanical Arm"),
            new(0x1F9BF, "Mechanical Leg"),
            new(0x1FAC0, "Anatomical Heart"),
            new(0x1FAC1, "Lungs"),
            new(0x1F9E0, "Brain"));

        public static readonly Emoji whiteFlower = new(0x1F4AE, "White Flower");
        public static readonly EmojiGroup plants = new(
            "Plants", "Flowers, trees, and things",
            new(0x1F331, "Seedling"),
            new(0x1F332, "Evergreen Tree"),
            new(0x1F333, "Deciduous Tree"),
            new(0x1F334, "Palm Tree"),
            new(0x1F335, "Cactus"),
            new(0x1F337, "Tulip"),
            new(0x1F338, "Cherry Blossom"),
            new(0x1F339, "Rose"),
            new(0x1F33A, "Hibiscus"),
            new(0x1F33B, "Sunflower"),
            new(0x1F33C, "Blossom"),
            sheafOfRice,
            new(0x1F33F, "Herb"),
            new(0x1F340, "Four Leaf Clover"),
            new(0x1F341, "Maple Leaf"),
            new(0x1F342, "Fallen Leaf"),
            new(0x1F343, "Leaf Fluttering in Wind"),
            new(0x1F3F5 + emojiStyle, "Rosette"),
            new(0x1F490, "Bouquet"),
            whiteFlower,
            new(0x1F940, "Wilted Flower"),
            new(0x1FAB4, "Potted Plant"),
            new("\u2618" + emojiStyle, "Shamrock"));

        public static readonly Emoji banana = new(0x1F34C, "Banana");
        public static readonly EmojiGroup food = new(
            "Food", "Food, drink, and utensils",
            new(0x1F32D, "Hot Dog"),
            new(0x1F32E, "Taco"),
            new(0x1F32F, "Burrito"),
            new(0x1F330, "Chestnut"),
            new(0x1F336 + emojiStyle, "Hot Pepper"),
            new(0x1F33D, "Ear of Corn"),
            new(0x1F344, "Mushroom"),
            new(0x1F345, "Tomato"),
            new(0x1F346, "Eggplant"),
            new(0x1F347, "Grapes"),
            new(0x1F348, "Melon"),
            new(0x1F349, "Watermelon"),
            new(0x1F34A, "Tangerine"),
            new(0x1F34B, "Lemon"),
            banana,
            new(0x1F34D, "Pineapple"),
            new(0x1F34E, "Red Apple"),
            new(0x1F34F, "Green Apple"),
            new(0x1F350, "Pear"),
            new(0x1F351, "Peach"),
            new(0x1F352, "Cherries"),
            new(0x1F353, "Strawberry"),
            new(0x1F354, "Hamburger"),
            new(0x1F355, "Pizza"),
            new(0x1F356, "Meat on Bone"),
            new(0x1F357, "Poultry Leg"),
            new(0x1F358, "Rice Cracker"),
            new(0x1F359, "Rice Ball"),
            new(0x1F35A, "Cooked Rice"),
            new(0x1F35B, "Curry Rice"),
            new(0x1F35C, "Steaming Bowl"),
            new(0x1F35D, "Spaghetti"),
            new(0x1F35E, "Bread"),
            new(0x1F35F, "French Fries"),
            new(0x1F360, "Roasted Sweet Potato"),
            new(0x1F361, "Dango"),
            new(0x1F362, "Oden"),
            new(0x1F363, "Sushi"),
            new(0x1F364, "Fried Shrimp"),
            new(0x1F365, "Fish Cake with Swirl"),
            new(0x1F371, "Bento Box"),
            new(0x1F372, "Pot of Food"),
            cooking,
            new(0x1F37F, "Popcorn"),
            new(0x1F950, "Croissant"),
            new(0x1F951, "Avocado"),
            new(0x1F952, "Cucumber"),
            new(0x1F953, "Bacon"),
            new(0x1F954, "Potato"),
            new(0x1F955, "Carrot"),
            new(0x1F956, "Baguette Bread"),
            new(0x1F957, "Green Salad"),
            new(0x1F958, "Shallow Pan of Food"),
            new(0x1F959, "Stuffed Flatbread"),
            new(0x1F95A, "Egg"),
            new(0x1F95C, "Peanuts"),
            new(0x1F95D, "Kiwi Fruit"),
            new(0x1F95E, "Pancakes"),
            new(0x1F95F, "Dumpling"),
            new(0x1F960, "Fortune Cookie"),
            new(0x1F961, "Takeout Box"),
            new(0x1F963, "Bowl with Spoon"),
            new(0x1F965, "Coconut"),
            new(0x1F966, "Broccoli"),
            new(0x1F968, "Pretzel"),
            new(0x1F969, "Cut of Meat"),
            new(0x1F96A, "Sandwich"),
            new(0x1F96B, "Canned Food"),
            new(0x1F96C, "Leafy Green"),
            new(0x1F96D, "Mango"),
            new(0x1F96E, "Moon Cake"),
            new(0x1F96F, "Bagel"),
            new(0x1F980, "Crab"),
            new(0x1F990, "Shrimp"),
            new(0x1F991, "Squid"),
            new(0x1F99E, "Lobster"),
            new(0x1F9AA, "Oyster"),
            new(0x1F9C0, "Cheese Wedge"),
            new(0x1F9C2, "Salt"),
            new(0x1F9C4, "Garlic"),
            new(0x1F9C5, "Onion"),
            new(0x1F9C6, "Falafel"),
            new(0x1F9C7, "Waffle"),
            new(0x1F9C8, "Butter"),
            new(0x1FAD0, "Blueberries"),
            new(0x1FAD1, "Bell Pepper"),
            new(0x1FAD2, "Olive"),
            new(0x1FAD3, "Flatbread"),
            new(0x1FAD4, "Tamale"),
            new(0x1FAD5, "Fondue"),
            new(0x1F366, "Soft Ice Cream"),
            new(0x1F367, "Shaved Ice"),
            new(0x1F368, "Ice Cream"),
            new(0x1F369, "Doughnut"),
            new(0x1F36A, "Cookie"),
            new(0x1F36B, "Chocolate Bar"),
            new(0x1F36C, "Candy"),
            new(0x1F36D, "Lollipop"),
            new(0x1F36E, "Custard"),
            new(0x1F36F, "Honey Pot"),
            new(0x1F370, "Shortcake"),
            new(0x1F382, "Birthday Cake"),
            new(0x1F967, "Pie"),
            new(0x1F9C1, "Cupcake"),
            new(0x1F375, "Teacup Without Handle"),
            new(0x1F376, "Sake"),
            new(0x1F377, "Wine Glass"),
            new(0x1F378, "Cocktail Glass"),
            new(0x1F379, "Tropical Drink"),
            new(0x1F37A, "Beer Mug"),
            new(0x1F37B, "Clinking Beer Mugs"),
            new(0x1F37C, "Baby Bottle"),
            new(0x1F37E, "Bottle with Popping Cork"),
            new(0x1F942, "Clinking Glasses"),
            new(0x1F943, "Tumbler Glass"),
            new(0x1F95B, "Glass of Milk"),
            new(0x1F964, "Cup with Straw"),
            new(0x1F9C3, "Beverage Box"),
            new(0x1F9C9, "Mate"),
            new(0x1F9CA, "Ice"),
            new(0x1F9CB, "Bubble Tea"),
            new(0x1FAD6, "Teapot"),
            new("\u2615", "Hot Beverage"),
            new(0x1F374, "Fork and Knife"),
            new(0x1F37D + emojiStyle, "Fork and Knife with Plate"),
            new(0x1F3FA, "Amphora"),
            new(0x1F52A, "Kitchen Knife"),
            new(0x1F944, "Spoon"),
            new(0x1F962, "Chopsticks"));

        public static readonly Emoji motorcycle = new(0x1F3CD + emojiStyle, "Motorcycle");
        public static readonly Emoji racingCar = new(0x1F3CE + emojiStyle, "Racing Car");
        public static readonly Emoji seat = new(0x1F4BA, "Seat");
        public static readonly Emoji helicopter = new(0x1F681, "Helicopter");
        public static readonly Emoji locomotive = new(0x1F682, "Locomotive");
        public static readonly Emoji railwayCar = new(0x1F683, "Railway Car");
        public static readonly Emoji highspeedTrain = new(0x1F684, "High-Speed Train");
        public static readonly Emoji bulletTrain = new(0x1F685, "Bullet Train");
        public static readonly Emoji train = new(0x1F686, "Train");
        public static readonly Emoji metro = new(0x1F687, "Metro");
        public static readonly Emoji lightRail = new(0x1F688, "Light Rail");
        public static readonly Emoji station = new(0x1F689, "Station");
        public static readonly Emoji tram = new(0x1F68A, "Tram");
        public static readonly Emoji tramCar = new(0x1F68B, "Tram Car");
        public static readonly Emoji bus = new(0x1F68C, "Bus");
        public static readonly Emoji oncomingBus = new(0x1F68D, "Oncoming Bus");
        public static readonly Emoji trolleyBus = new(0x1F68E, "Trolleybus");
        public static readonly Emoji busStop = new(0x1F68F, "Bus Stop");
        public static readonly Emoji miniBus = new(0x1F690, "Minibus");
        public static readonly Emoji ambulance = new(0x1F691, "Ambulance");
        public static readonly Emoji policeCar = new(0x1F693, "Police Car");
        public static readonly Emoji oncomingPoliceCar = new(0x1F694, "Oncoming Police Car");
        public static readonly Emoji taxi = new(0x1F695, "Taxi");
        public static readonly Emoji oncomingTaxi = new(0x1F696, "Oncoming Taxi");
        public static readonly Emoji automobile = new(0x1F697, "Automobile");
        public static readonly Emoji oncomingAutomobile = new(0x1F698, "Oncoming Automobile");
        public static readonly Emoji sportUtilityVehicle = new(0x1F699, "Sport Utility Vehicle");
        public static readonly Emoji deliveryTruck = new(0x1F69A, "Delivery Truck");
        public static readonly Emoji articulatedLorry = new(0x1F69B, "Articulated Lorry");
        public static readonly Emoji tractor = new(0x1F69C, "Tractor");
        public static readonly Emoji monorail = new(0x1F69D, "Monorail");
        public static readonly Emoji mountainRailway = new(0x1F69E, "Mountain Railway");
        public static readonly Emoji suspensionRailway = new(0x1F69F, "Suspension Railway");
        public static readonly Emoji mountainCableway = new(0x1F6A0, "Mountain Cableway");
        public static readonly Emoji aerialTramway = new(0x1F6A1, "Aerial Tramway");
        public static readonly Emoji ship = new(0x1F6A2, "Ship");
        public static readonly Emoji speedBoat = new(0x1F6A4, "Speedboat");
        public static readonly Emoji horizontalTrafficLight = new(0x1F6A5, "Horizontal Traffic Light");
        public static readonly Emoji verticalTrafficLight = new(0x1F6A6, "Vertical Traffic Light");
        public static readonly Emoji construction = new(0x1F6A7, "Construction");
        public static readonly Emoji policeCarLight = new(0x1F6A8, "Police Car Light");
        public static readonly Emoji bicycle = new(0x1F6B2, "Bicycle");
        public static readonly Emoji stopSign = new(0x1F6D1, "Stop Sign");
        public static readonly Emoji oilDrum = new(0x1F6E2 + emojiStyle, "Oil Drum");
        public static readonly Emoji motorway = new(0x1F6E3 + emojiStyle, "Motorway");
        public static readonly Emoji railwayTrack = new(0x1F6E4 + emojiStyle, "Railway Track");
        public static readonly Emoji motorBoat = new(0x1F6E5 + emojiStyle, "Motor Boat");
        public static readonly Emoji smallAirplane = new(0x1F6E9 + emojiStyle, "Small Airplane");
        public static readonly Emoji airplaneDeparture = new(0x1F6EB, "Airplane Departure");
        public static readonly Emoji airplaneArrival = new(0x1F6EC, "Airplane Arrival");
        public static readonly Emoji satellite = new(0x1F6F0 + emojiStyle, "Satellite");
        public static readonly Emoji passengerShip = new(0x1F6F3 + emojiStyle, "Passenger Ship");
        public static readonly Emoji kickScooter = new(0x1F6F4, "Kick Scooter");
        public static readonly Emoji motorScooter = new(0x1F6F5, "Motor Scooter");
        public static readonly Emoji canoe = new(0x1F6F6, "Canoe");
        public static readonly Emoji flyingSaucer = new(0x1F6F8, "Flying Saucer");
        public static readonly Emoji skateboard = new(0x1F6F9, "Skateboard");
        public static readonly Emoji autoRickshaw = new(0x1F6FA, "Auto Rickshaw");
        public static readonly Emoji pickupTruck = new(0x1F6FB, "Pickup Truck");
        public static readonly Emoji rollerSkate = new(0x1F6FC, "Roller Skate");
        public static readonly Emoji parachute = new(0x1FA82, "Parachute");
        public static readonly Emoji anchor = new("\u2693", "Anchor");
        public static readonly Emoji ferry = new("\u26F4" + emojiStyle, "Ferry");
        public static readonly Emoji sailboat = new("\u26F5", "Sailboat");
        public static readonly Emoji fuelPump = new("\u26FD", "Fuel Pump");
        public static readonly EmojiGroup vehicles = new(
            "Vehicles", "Things that go",
            motorcycle,
            racingCar,
            seat,
            rocket,
            helicopter,
            locomotive,
            railwayCar,
            highspeedTrain,
            bulletTrain,
            train,
            metro,
            lightRail,
            station,
            tram,
            tramCar,
            bus,
            oncomingBus,
            trolleyBus,
            busStop,
            miniBus,
            ambulance,
            fireEngine,
            taxi,
            oncomingTaxi,
            automobile,
            oncomingAutomobile,
            sportUtilityVehicle,
            deliveryTruck,
            articulatedLorry,
            tractor,
            monorail,
            mountainRailway,
            suspensionRailway,
            mountainCableway,
            aerialTramway,
            ship,
            speedBoat,
            horizontalTrafficLight,
            verticalTrafficLight,
            construction,
            bicycle,
            stopSign,
            oilDrum,
            motorway,
            railwayTrack,
            motorBoat,
            smallAirplane,
            airplaneDeparture,
            airplaneArrival,
            satellite,
            passengerShip,
            kickScooter,
            motorScooter,
            canoe,
            flyingSaucer,
            skateboard,
            autoRickshaw,
            pickupTruck,
            rollerSkate,
            motorizedWheelchair,
            manualWheelchair,
            parachute,
            anchor,
            ferry,
            sailboat,
            fuelPump,
            airplane);
        private static readonly Emoji bloodTypeButtonA = new(0x1F170, "A Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonB = new(0x1F171, "B Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonO = new(0x1F17E, "O Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonAB = new(0x1F18E, "AB Button (Blood Type)");
        public static readonly EmojiGroup bloodTypes = new(
            "Blood Types", "Blood types",
            bloodTypeButtonA,
            bloodTypeButtonB,
            bloodTypeButtonO,
            bloodTypeButtonAB);

        public static readonly Emoji regionalIndicatorSymbolLetterA = new(0x1F1E6, "Regional Indicator Symbol Letter A");
        public static readonly Emoji regionalIndicatorSymbolLetterB = new(0x1F1E7, "Regional Indicator Symbol Letter B");
        public static readonly Emoji regionalIndicatorSymbolLetterC = new(0x1F1E8, "Regional Indicator Symbol Letter C");
        public static readonly Emoji regionalIndicatorSymbolLetterD = new(0x1F1E9, "Regional Indicator Symbol Letter D");
        public static readonly Emoji regionalIndicatorSymbolLetterE = new(0x1F1EA, "Regional Indicator Symbol Letter E");
        public static readonly Emoji regionalIndicatorSymbolLetterF = new(0x1F1EB, "Regional Indicator Symbol Letter F");
        public static readonly Emoji regionalIndicatorSymbolLetterG = new(0x1F1EC, "Regional Indicator Symbol Letter G");
        public static readonly Emoji regionalIndicatorSymbolLetterH = new(0x1F1ED, "Regional Indicator Symbol Letter H");
        public static readonly Emoji regionalIndicatorSymbolLetterI = new(0x1F1EE, "Regional Indicator Symbol Letter I");
        public static readonly Emoji regionalIndicatorSymbolLetterJ = new(0x1F1EF, "Regional Indicator Symbol Letter J");
        public static readonly Emoji regionalIndicatorSymbolLetterK = new(0x1F1F0, "Regional Indicator Symbol Letter K");
        public static readonly Emoji regionalIndicatorSymbolLetterL = new(0x1F1F1, "Regional Indicator Symbol Letter L");
        public static readonly Emoji regionalIndicatorSymbolLetterM = new(0x1F1F2, "Regional Indicator Symbol Letter M");
        public static readonly Emoji regionalIndicatorSymbolLetterN = new(0x1F1F3, "Regional Indicator Symbol Letter N");
        public static readonly Emoji regionalIndicatorSymbolLetterO = new(0x1F1F4, "Regional Indicator Symbol Letter O");
        public static readonly Emoji regionalIndicatorSymbolLetterP = new(0x1F1F5, "Regional Indicator Symbol Letter P");
        public static readonly Emoji regionalIndicatorSymbolLetterQ = new(0x1F1F6, "Regional Indicator Symbol Letter Q");
        public static readonly Emoji regionalIndicatorSymbolLetterR = new(0x1F1F7, "Regional Indicator Symbol Letter R");
        public static readonly Emoji regionalIndicatorSymbolLetterS = new(0x1F1F8, "Regional Indicator Symbol Letter S");
        public static readonly Emoji regionalIndicatorSymbolLetterT = new(0x1F1F9, "Regional Indicator Symbol Letter T");
        public static readonly Emoji regionalIndicatorSymbolLetterU = new(0x1F1FA, "Regional Indicator Symbol Letter U");
        public static readonly Emoji regionalIndicatorSymbolLetterV = new(0x1F1FB, "Regional Indicator Symbol Letter V");
        public static readonly Emoji regionalIndicatorSymbolLetterW = new(0x1F1FC, "Regional Indicator Symbol Letter W");
        public static readonly Emoji regionalIndicatorSymbolLetterX = new(0x1F1FD, "Regional Indicator Symbol Letter X");
        public static readonly Emoji regionalIndicatorSymbolLetterY = new(0x1F1FE, "Regional Indicator Symbol Letter Y");
        public static readonly Emoji regionalIndicatorSymbolLetterZ = new(0x1F1FF, "Regional Indicator Symbol Letter Z");
        public static readonly EmojiGroup regionIndicators = new(
            "Regions", "Region indicators",
            regionalIndicatorSymbolLetterA,
            regionalIndicatorSymbolLetterB,
            regionalIndicatorSymbolLetterC,
            regionalIndicatorSymbolLetterD,
            regionalIndicatorSymbolLetterE,
            regionalIndicatorSymbolLetterF,
            regionalIndicatorSymbolLetterG,
            regionalIndicatorSymbolLetterH,
            regionalIndicatorSymbolLetterI,
            regionalIndicatorSymbolLetterJ,
            regionalIndicatorSymbolLetterK,
            regionalIndicatorSymbolLetterL,
            regionalIndicatorSymbolLetterM,
            regionalIndicatorSymbolLetterN,
            regionalIndicatorSymbolLetterO,
            regionalIndicatorSymbolLetterP,
            regionalIndicatorSymbolLetterQ,
            regionalIndicatorSymbolLetterR,
            regionalIndicatorSymbolLetterS,
            regionalIndicatorSymbolLetterT,
            regionalIndicatorSymbolLetterU,
            regionalIndicatorSymbolLetterV,
            regionalIndicatorSymbolLetterW,
            regionalIndicatorSymbolLetterX,
            regionalIndicatorSymbolLetterY,
            regionalIndicatorSymbolLetterZ);

        public static readonly EmojiGroup japanese = new(
            "Japanese", "Japanse symbology",
            new(0x1F530, "Japanese Symbol for Beginner"),
            new(0x1F201, "Japanese “Here” Button"),
            new(0x1F202 + emojiStyle, "Japanese “Service Charge” Button"),
            new(0x1F21A, "Japanese “Free of Charge” Button"),
            new(0x1F22F, "Japanese “Reserved” Button"),
            new(0x1F232, "Japanese “Prohibited” Button"),
            new(0x1F233, "Japanese “Vacancy” Button"),
            new(0x1F234, "Japanese “Passing Grade” Button"),
            new(0x1F235, "Japanese “No Vacancy” Button"),
            new(0x1F236, "Japanese “Not Free of Charge” Button"),
            new(0x1F237 + emojiStyle, "Japanese “Monthly Amount” Button"),
            new(0x1F238, "Japanese “Application” Button"),
            new(0x1F239, "Japanese “Discount” Button"),
            new(0x1F23A, "Japanese “Open for Business” Button"),
            new(0x1F250, "Japanese “Bargain” Button"),
            new(0x1F251, "Japanese “Acceptable” Button"),
            new("\u3297" + emojiStyle, "Japanese “Congratulations” Button"),
            new("\u3299" + emojiStyle, "Japanese “Secret” Button"));

        public static readonly EmojiGroup clocks = new(
            "Clocks", "Time-keeping pieces",
            new(0x1F550, "One O’Clock"),
            new(0x1F551, "Two O’Clock"),
            new(0x1F552, "Three O’Clock"),
            new(0x1F553, "Four O’Clock"),
            new(0x1F554, "Five O’Clock"),
            new(0x1F555, "Six O’Clock"),
            new(0x1F556, "Seven O’Clock"),
            new(0x1F557, "Eight O’Clock"),
            new(0x1F558, "Nine O’Clock"),
            new(0x1F559, "Ten O’Clock"),
            new(0x1F55A, "Eleven O’Clock"),
            new(0x1F55B, "Twelve O’Clock"),
            new(0x1F55C, "One-Thirty"),
            new(0x1F55D, "Two-Thirty"),
            new(0x1F55E, "Three-Thirty"),
            new(0x1F55F, "Four-Thirty"),
            new(0x1F560, "Five-Thirty"),
            new(0x1F561, "Six-Thirty"),
            new(0x1F562, "Seven-Thirty"),
            new(0x1F563, "Eight-Thirty"),
            new(0x1F564, "Nine-Thirty"),
            new(0x1F565, "Ten-Thirty"),
            new(0x1F566, "Eleven-Thirty"),
            new(0x1F567, "Twelve-Thirty"),
            new(0x1F570 + emojiStyle, "Mantelpiece Clock"),
            new("\u231A", "Watch"),
            new("\u23F0", "Alarm Clock"),
            new("\u23F1" + emojiStyle, "Stopwatch"),
            new("\u23F2" + emojiStyle, "Timer Clock"),
            new("\u231B", "Hourglass Done"),
            new("\u23F3", "Hourglass Not Done"));

        public static readonly Emoji clockwiseVerticalArrows = new(0x1F503 + emojiStyle, "Clockwise Vertical Arrows");
        public static readonly Emoji counterclockwiseArrowsButton = new(0x1F504 + emojiStyle, "Counterclockwise Arrows Button");
        public static readonly Emoji leftRightArrow = new("\u2194" + emojiStyle, "Left-Right Arrow");
        public static readonly Emoji upDownArrow = new("\u2195" + emojiStyle, "Up-Down Arrow");
        public static readonly Emoji upLeftArrow = new("\u2196" + emojiStyle, "Up-Left Arrow");
        public static readonly Emoji upRightArrow = new("\u2197" + emojiStyle, "Up-Right Arrow");
        public static readonly Emoji downRightArrow = new("\u2198", "Down-Right Arrow");
        public static readonly Emoji downRightArrowText = new("\u2198" + textStyle, "Down-Right Arrow");
        public static readonly Emoji downRightArrowEmoji = new("\u2198" + emojiStyle, "Down-Right Arrow");
        public static readonly Emoji downLeftArrow = new("\u2199" + emojiStyle, "Down-Left Arrow");
        public static readonly Emoji rightArrowCurvingLeft = new("\u21A9" + emojiStyle, "Right Arrow Curving Left");
        public static readonly Emoji leftArrowCurvingRight = new("\u21AA" + emojiStyle, "Left Arrow Curving Right");
        public static readonly Emoji rightArrow = new("\u27A1" + emojiStyle, "Right Arrow");
        public static readonly Emoji rightArrowCurvingUp = new("\u2934" + emojiStyle, "Right Arrow Curving Up");
        public static readonly Emoji rightArrowCurvingDown = new("\u2935" + emojiStyle, "Right Arrow Curving Down");
        public static readonly Emoji leftArrow = new("\u2B05" + emojiStyle, "Left Arrow");
        public static readonly Emoji upArrow = new("\u2B06" + emojiStyle, "Up Arrow");
        public static readonly Emoji downArrow = new("\u2B07" + emojiStyle, "Down Arrow");
        public static readonly EmojiGroup arrows = new(
            "Arrows", "Arrows pointing in different directions",
            clockwiseVerticalArrows,
            counterclockwiseArrowsButton,
            leftRightArrow,
            upDownArrow,
            upLeftArrow,
            upRightArrow,
            downRightArrowEmoji,
            downLeftArrow,
            rightArrowCurvingLeft,
            leftArrowCurvingRight,
            rightArrow,
            rightArrowCurvingUp,
            rightArrowCurvingDown,
            leftArrow,
            upArrow,
            downArrow);

        public static readonly Emoji redCircle = new(0x1F534, "Red Circle");
        public static readonly Emoji blueCircle = new(0x1F535, "Blue Circle");
        public static readonly Emoji largeOrangeDiamond = new(0x1F536, "Large Orange Diamond");
        public static readonly Emoji largeBlueDiamond = new(0x1F537, "Large Blue Diamond");
        public static readonly Emoji smallOrangeDiamond = new(0x1F538, "Small Orange Diamond");
        public static readonly Emoji smallBlueDiamond = new(0x1F539, "Small Blue Diamond");
        public static readonly Emoji redTrianglePointedUp = new(0x1F53A, "Red Triangle Pointed Up");
        public static readonly Emoji redTrianglePointedDown = new(0x1F53B, "Red Triangle Pointed Down");
        public static readonly Emoji orangeCircle = new(0x1F7E0, "Orange Circle");
        public static readonly Emoji yellowCircle = new(0x1F7E1, "Yellow Circle");
        public static readonly Emoji greenCircle = new(0x1F7E2, "Green Circle");
        public static readonly Emoji purpleCircle = new(0x1F7E3, "Purple Circle");
        public static readonly Emoji brownCircle = new(0x1F7E4, "Brown Circle");
        public static readonly Emoji hollowRedCircle = new("\u2B55", "Hollow Red Circle");
        public static readonly Emoji whiteCircle = new("\u26AA", "White Circle");
        public static readonly Emoji blackCircle = new("\u26AB", "Black Circle");
        public static readonly Emoji redSquare = new(0x1F7E5, "Red Square");
        public static readonly Emoji blueSquare = new(0x1F7E6, "Blue Square");
        public static readonly Emoji orangeSquare = new(0x1F7E7, "Orange Square");
        public static readonly Emoji yellowSquare = new(0x1F7E8, "Yellow Square");
        public static readonly Emoji greenSquare = new(0x1F7E9, "Green Square");
        public static readonly Emoji purpleSquare = new(0x1F7EA, "Purple Square");
        public static readonly Emoji brownSquare = new(0x1F7EB, "Brown Square");
        public static readonly Emoji blackSquareButton = new(0x1F532, "Black Square Button");
        public static readonly Emoji whiteSquareButton = new(0x1F533, "White Square Button");
        public static readonly Emoji blackSmallSquare = new("\u25AA" + emojiStyle, "Black Small Square");
        public static readonly Emoji whiteSmallSquare = new("\u25AB" + emojiStyle, "White Small Square");
        public static readonly Emoji whiteMediumSmallSquare = new("\u25FD", "White Medium-Small Square");
        public static readonly Emoji blackMediumSmallSquare = new("\u25FE", "Black Medium-Small Square");
        public static readonly Emoji whiteMediumSquare = new("\u25FB" + emojiStyle, "White Medium Square");
        public static readonly Emoji blackMediumSquare = new("\u25FC" + emojiStyle, "Black Medium Square");
        public static readonly Emoji blackLargeSquare = new("\u2B1B", "Black Large Square");
        public static readonly Emoji whiteLargeSquare = new("\u2B1C", "White Large Square");
        public static readonly Emoji star = new("\u2B50", "Star");
        public static readonly Emoji diamondWithADot = new(0x1F4A0, "Diamond with a Dot");
        public static readonly EmojiGroup shapes = new(
            "Shapes", "Colored shapes",
            redCircle,
            blueCircle,
            largeOrangeDiamond,
            largeBlueDiamond,
            smallOrangeDiamond,
            smallBlueDiamond,
            redTrianglePointedUp,
            redTrianglePointedDown,
            orangeCircle,
            yellowCircle,
            greenCircle,
            purpleCircle,
            brownCircle,
            hollowRedCircle,
            whiteCircle,
            blackCircle,
            redSquare,
            blueSquare,
            orangeSquare,
            yellowSquare,
            greenSquare,
            purpleSquare,
            brownSquare,
            blackSquareButton,
            whiteSquareButton,
            blackSmallSquare,
            whiteSmallSquare,
            whiteMediumSmallSquare,
            blackMediumSmallSquare,
            whiteMediumSquare,
            blackMediumSquare,
            blackLargeSquare,
            whiteLargeSquare,
            star,
            diamondWithADot);

        public static readonly Emoji clearButton = new(0x1F191, "CL Button");
        public static readonly Emoji coolButton = new(0x1F192, "Cool Button");
        public static readonly Emoji freeButton = new(0x1F193, "Free Button");
        public static readonly Emoji idButton = new(0x1F194, "ID Button");
        public static readonly Emoji newButton = new(0x1F195, "New Button");
        public static readonly Emoji ngButton = new(0x1F196, "NG Button");
        public static readonly Emoji okButton = new(0x1F197, "OK Button");
        public static readonly Emoji sosButton = new(0x1F198, "SOS Button");
        public static readonly Emoji upButton = new(0x1F199, "Up! Button");
        public static readonly Emoji vsButton = new(0x1F19A, "Vs Button");
        public static readonly Emoji radioButton = new(0x1F518, "Radio Button");
        public static readonly Emoji backArrow = new(0x1F519, "Back Arrow");
        public static readonly Emoji endArrow = new(0x1F51A, "End Arrow");
        public static readonly Emoji onArrow = new(0x1F51B, "On! Arrow");
        public static readonly Emoji soonArrow = new(0x1F51C, "Soon Arrow");
        public static readonly Emoji topArrow = new(0x1F51D, "Top Arrow");
        public static readonly Emoji checkBoxWithCheck = new("\u2611" + emojiStyle, "Check Box with Check");
        public static readonly Emoji inputLatinUppercase = new(0x1F520, "Input Latin Uppercase");
        public static readonly Emoji inputLatinLowercase = new(0x1F521, "Input Latin Lowercase");
        public static readonly Emoji inputNumbers = new(0x1F522, "Input Numbers");
        public static readonly Emoji inputSymbols = new(0x1F523, "Input Symbols");
        public static readonly Emoji inputLatinLetters = new(0x1F524, "Input Latin Letters");
        public static readonly Emoji shuffleTracksButton = new(0x1F500, "Shuffle Tracks Button");
        public static readonly Emoji repeatButton = new(0x1F501, "Repeat Button");
        public static readonly Emoji repeatSingleButton = new(0x1F502, "Repeat Single Button");
        public static readonly Emoji upwardsButton = new(0x1F53C, "Upwards Button");
        public static readonly Emoji downwardsButton = new(0x1F53D, "Downwards Button");
        public static readonly Emoji playButton = new("\u25B6" + emojiStyle, "Play Button");
        public static readonly Emoji reverseButton = new("\u25C0" + emojiStyle, "Reverse Button");
        public static readonly Emoji ejectButton = new("\u23CF" + emojiStyle, "Eject Button");
        public static readonly Emoji fastForwardButton = new("\u23E9", "Fast-Forward Button");
        public static readonly Emoji fastReverseButton = new("\u23EA", "Fast Reverse Button");
        public static readonly Emoji fastUpButton = new("\u23EB", "Fast Up Button");
        public static readonly Emoji fastDownButton = new("\u23EC", "Fast Down Button");
        public static readonly Emoji nextTrackButton = new("\u23ED" + emojiStyle, "Next Track Button");
        public static readonly Emoji lastTrackButton = new("\u23EE" + emojiStyle, "Last Track Button");
        public static readonly Emoji playOrPauseButton = new("\u23EF" + emojiStyle, "Play or Pause Button");
        public static readonly Emoji pauseButton = new("\u23F8" + emojiStyle, "Pause Button");
        public static readonly Emoji stopButton = new("\u23F9" + emojiStyle, "Stop Button");
        public static readonly Emoji recordButton = new("\u23FA" + emojiStyle, "Record Button");
        public static readonly EmojiGroup buttons = new(
            "Buttons", "Buttons",
            clearButton,
            coolButton,
            freeButton,
            idButton,
            newButton,
            ngButton,
            okButton,
            sosButton,
            upButton,
            vsButton,
            radioButton,
            backArrow,
            endArrow,
            onArrow,
            soonArrow,
            topArrow,
            checkBoxWithCheck,
            inputLatinUppercase,
            inputLatinLowercase,
            inputNumbers,
            inputSymbols,
            inputLatinLetters,
            shuffleTracksButton,
            repeatButton,
            repeatSingleButton,
            upwardsButton,
            downwardsButton,
            playButton,
            pauseButton,
            reverseButton,
            ejectButton,
            fastForwardButton,
            fastReverseButton,
            fastUpButton,
            fastDownButton,
            nextTrackButton,
            lastTrackButton,
            playOrPauseButton,
            pauseButton,
            stopButton,
            recordButton);

        public static readonly EmojiGroup zodiac = new(
            "Zodiac", "The symbology of astrology",
            new("\u2648", "Aries"),
            new("\u2649", "Taurus"),
            new("\u264A", "Gemini"),
            new("\u264B", "Cancer"),
            new("\u264C", "Leo"),
            new("\u264D", "Virgo"),
            new("\u264E", "Libra"),
            new("\u264F", "Scorpio"),
            new("\u2650", "Sagittarius"),
            new("\u2651", "Capricorn"),
            new("\u2652", "Aquarius"),
            new("\u2653", "Pisces"),
            new("\u26CE", "Ophiuchus"));

        public static readonly EmojiGroup numbers = new(
            "Numbers", "Numbers",
            new("0" + emojiStyle, "Digit Zero"),
            new("1" + emojiStyle, "Digit One"),
            new("2" + emojiStyle, "Digit Two"),
            new("3" + emojiStyle, "Digit Three"),
            new("4" + emojiStyle, "Digit Four"),
            new("5" + emojiStyle, "Digit Five"),
            new("6" + emojiStyle, "Digit Six"),
            new("7" + emojiStyle, "Digit Seven"),
            new("8" + emojiStyle, "Digit Eight"),
            new("9" + emojiStyle, "Digit Nine"),
            new("*" + emojiStyle, "Asterisk"),
            new("#" + emojiStyle, "Number Sign"),
            new("0" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Zero"),
            new("1" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit One"),
            new("2" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Two"),
            new("3" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Three"),
            new("4" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Four"),
            new("5" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Five"),
            new("6" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Six"),
            new("7" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Seven"),
            new("8" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Eight"),
            new("9" + emojiStyle + combiningEnclosingKeycap, "Keycap Digit Nine"),
            new("*" + emojiStyle + combiningEnclosingKeycap, "Keycap Asterisk"),
            new("#" + emojiStyle + combiningEnclosingKeycap, "Keycap Number Sign"),
            new(0x1F51F, "Keycap: 10"));

        public static readonly Emoji tagPlusSign = new(0xE002B, "Tag Plus Sign");
        public static readonly Emoji tagMinusHyphen = new(0xE002D, "Tag Hyphen-Minus");
        public static readonly Emoji tagLatinSmallLetterA = new(0xE0061, "Tag Latin Small Letter A");
        public static readonly Emoji tagLatinSmallLetterB = new(0xE0062, "Tag Latin Small Letter B");
        public static readonly Emoji tagLatinSmallLetterC = new(0xE0063, "Tag Latin Small Letter C");
        public static readonly Emoji tagLatinSmallLetterD = new(0xE0064, "Tag Latin Small Letter D");
        public static readonly Emoji tagLatinSmallLetterE = new(0xE0065, "Tag Latin Small Letter E");
        public static readonly Emoji tagLatinSmallLetterF = new(0xE0066, "Tag Latin Small Letter F");
        public static readonly Emoji tagLatinSmallLetterG = new(0xE0067, "Tag Latin Small Letter G");
        public static readonly Emoji tagLatinSmallLetterH = new(0xE0068, "Tag Latin Small Letter H");
        public static readonly Emoji tagLatinSmallLetterI = new(0xE0069, "Tag Latin Small Letter I");
        public static readonly Emoji tagLatinSmallLetterJ = new(0xE006A, "Tag Latin Small Letter J");
        public static readonly Emoji tagLatinSmallLetterK = new(0xE006B, "Tag Latin Small Letter K");
        public static readonly Emoji tagLatinSmallLetterL = new(0xE006C, "Tag Latin Small Letter L");
        public static readonly Emoji tagLatinSmallLetterM = new(0xE006D, "Tag Latin Small Letter M");
        public static readonly Emoji tagLatinSmallLetterN = new(0xE006E, "Tag Latin Small Letter N");
        public static readonly Emoji tagLatinSmallLetterO = new(0xE006F, "Tag Latin Small Letter O");
        public static readonly Emoji tagLatinSmallLetterP = new(0xE0070, "Tag Latin Small Letter P");
        public static readonly Emoji tagLatinSmallLetterQ = new(0xE0071, "Tag Latin Small Letter Q");
        public static readonly Emoji tagLatinSmallLetterR = new(0xE0072, "Tag Latin Small Letter R");
        public static readonly Emoji tagLatinSmallLetterS = new(0xE0073, "Tag Latin Small Letter S");
        public static readonly Emoji tagLatinSmallLetterT = new(0xE0074, "Tag Latin Small Letter T");
        public static readonly Emoji tagLatinSmallLetterU = new(0xE0075, "Tag Latin Small Letter U");
        public static readonly Emoji tagLatinSmallLetterV = new(0xE0076, "Tag Latin Small Letter V");
        public static readonly Emoji tagLatinSmallLetterW = new(0xE0077, "Tag Latin Small Letter W");
        public static readonly Emoji tagLatinSmallLetterX = new(0xE0078, "Tag Latin Small Letter X");
        public static readonly Emoji tagLatinSmallLetterY = new(0xE0079, "Tag Latin Small Letter Y");
        public static readonly Emoji tagLatinSmallLetterZ = new(0xE007A, "Tag Latin Small Letter Z");
        public static readonly Emoji cancelTag = new(0xE007F, "Cancel Tag");
        public static readonly EmojiGroup tags = new(
            "Tags", "Tags",
            new(0xE0020, "Tag Space"),
            new(0xE0021, "Tag Exclamation Mark"),
            new(0xE0022, "Tag Quotation Mark"),
            new(0xE0023, "Tag Number Sign"),
            new(0xE0024, "Tag Dollar Sign"),
            new(0xE0025, "Tag Percent Sign"),
            new(0xE0026, "Tag Ampersand"),
            new(0xE0027, "Tag Apostrophe"),
            new(0xE0028, "Tag Left Parenthesis"),
            new(0xE0029, "Tag Right Parenthesis"),
            new(0xE002A, "Tag Asterisk"),
            tagPlusSign,
            new(0xE002C, "Tag Comma"),
            tagMinusHyphen,
            new(0xE002E, "Tag Full Stop"),
            new(0xE002F, "Tag Solidus"),
            new(0xE0030, "Tag Digit Zero"),
            new(0xE0031, "Tag Digit One"),
            new(0xE0032, "Tag Digit Two"),
            new(0xE0033, "Tag Digit Three"),
            new(0xE0034, "Tag Digit Four"),
            new(0xE0035, "Tag Digit Five"),
            new(0xE0036, "Tag Digit Six"),
            new(0xE0037, "Tag Digit Seven"),
            new(0xE0038, "Tag Digit Eight"),
            new(0xE0039, "Tag Digit Nine"),
            new(0xE003A, "Tag Colon"),
            new(0xE003B, "Tag Semicolon"),
            new(0xE003C, "Tag Less-Than Sign"),
            new(0xE003D, "Tag Equals Sign"),
            new(0xE003E, "Tag Greater-Than Sign"),
            new(0xE003F, "Tag Question Mark"),
            new(0xE0040, "Tag Commercial at"),
            new(0xE0041, "Tag Latin Capital Letter a"),
            new(0xE0042, "Tag Latin Capital Letter B"),
            new(0xE0043, "Tag Latin Capital Letter C"),
            new(0xE0044, "Tag Latin Capital Letter D"),
            new(0xE0045, "Tag Latin Capital Letter E"),
            new(0xE0046, "Tag Latin Capital Letter F"),
            new(0xE0047, "Tag Latin Capital Letter G"),
            new(0xE0048, "Tag Latin Capital Letter H"),
            new(0xE0049, "Tag Latin Capital Letter I"),
            new(0xE004A, "Tag Latin Capital Letter J"),
            new(0xE004B, "Tag Latin Capital Letter K"),
            new(0xE004C, "Tag Latin Capital Letter L"),
            new(0xE004D, "Tag Latin Capital Letter M"),
            new(0xE004E, "Tag Latin Capital Letter N"),
            new(0xE004F, "Tag Latin Capital Letter O"),
            new(0xE0050, "Tag Latin Capital Letter P"),
            new(0xE0051, "Tag Latin Capital Letter Q"),
            new(0xE0052, "Tag Latin Capital Letter R"),
            new(0xE0053, "Tag Latin Capital Letter S"),
            new(0xE0054, "Tag Latin Capital Letter T"),
            new(0xE0055, "Tag Latin Capital Letter U"),
            new(0xE0056, "Tag Latin Capital Letter V"),
            new(0xE0057, "Tag Latin Capital Letter W"),
            new(0xE0058, "Tag Latin Capital Letter X"),
            new(0xE0059, "Tag Latin Capital Letter Y"),
            new(0xE005A, "Tag Latin Capital Letter Z"),
            new(0xE005B, "Tag Left Square Bracket"),
            new(0xE005C, "Tag Reverse Solidus"),
            new(0xE005D, "Tag Right Square Bracket"),
            new(0xE005E, "Tag Circumflex Accent"),
            new(0xE005F, "Tag Low Line"),
            new(0xE0060, "Tag Grave Accent"),
            tagLatinSmallLetterA,
            tagLatinSmallLetterB,
            tagLatinSmallLetterC,
            tagLatinSmallLetterD,
            tagLatinSmallLetterE,
            tagLatinSmallLetterF,
            tagLatinSmallLetterG,
            tagLatinSmallLetterH,
            tagLatinSmallLetterI,
            tagLatinSmallLetterJ,
            tagLatinSmallLetterK,
            tagLatinSmallLetterL,
            tagLatinSmallLetterM,
            tagLatinSmallLetterN,
            tagLatinSmallLetterO,
            tagLatinSmallLetterP,
            tagLatinSmallLetterQ,
            tagLatinSmallLetterR,
            tagLatinSmallLetterS,
            tagLatinSmallLetterT,
            tagLatinSmallLetterU,
            tagLatinSmallLetterV,
            tagLatinSmallLetterW,
            tagLatinSmallLetterX,
            tagLatinSmallLetterY,
            tagLatinSmallLetterZ,
            new(0xE007B, "Tag Left Curly Bracket"),
            new(0xE007C, "Tag Vertical Line"),
            new(0xE007D, "Tag Right Curly Bracket"),
            new(0xE007E, "Tag Tilde"),
            cancelTag);

        public static readonly Emoji multiply = new("\u2716", "Multiply");
        public static readonly Emoji heavyPlus = new("\u2795", "Plus");
        public static readonly Emoji heavyMinus = new("\u2796", "Minus");
        public static readonly Emoji divide = new("\u2797", "Divide");
        public static readonly Emoji identical = new("\u2261", "Identical");
        public static readonly EmojiGroup math = new(
            "Math", "Math",
            multiply,
            heavyPlus,
            heavyMinus,
            divide,
            identical);

        public static readonly EmojiGroup games = new(
            "Games", "Games",
            new("\u2660" + emojiStyle, "Spade Suit"),
            new("\u2663" + emojiStyle, "Club Suit"),
            new("\u2665" + emojiStyle, "Heart Suit"),
            new("\u2666" + emojiStyle, "Diamond Suit"),
            new(0x1F004, "Mahjong Red Dragon"),
            new(0x1F0CF, "Joker"),
            new(0x1F3AF, "Direct Hit"),
            new(0x1F3B0, "Slot Machine"),
            new(0x1F3B1, "Pool 8 Ball"),
            new(0x1F3B2, "Game Die"),
            new(0x1F3B3, "Bowling"),
            new(0x1F3B4, "Flower Playing Cards"),
            new(0x1F9E9, "Puzzle Piece"),
            new("\u265F" + emojiStyle, "Chess Pawn"),
            new(0x1FA80, "Yo-Yo"),
            new(0x1FA83, "Boomerang"),
            new(0x1FA86, "Nesting Dolls"),
            new(0x1FA81, "Kite"));

        public static readonly EmojiGroup sportsEquipment = new(
            "Sports Equipment", "Sports equipment",
            new(0x1F3BD, "Running Shirt"),
            new(0x1F3BE, "Tennis"),
            new(0x1F3BF, "Skis"),
            new(0x1F3C0, "Basketball"),
            new(0x1F3C5, "Sports Medal"),
            new(0x1F3C6, "Trophy"),
            new(0x1F3C8, "American Football"),
            new(0x1F3C9, "Rugby Football"),
            new(0x1F3CF, "Cricket Game"),
            new(0x1F3D0, "Volleyball"),
            new(0x1F3D1, "Field Hockey"),
            new(0x1F3D2, "Ice Hockey"),
            new(0x1F3D3, "Ping Pong"),
            new(0x1F3F8, "Badminton"),
            new(0x1F6F7, "Sled"),
            new(0x1F945, "Goal Net"),
            new(0x1F947, "1st Place Medal"),
            new(0x1F948, "2nd Place Medal"),
            new(0x1F949, "3rd Place Medal"),
            new(0x1F94A, "Boxing Glove"),
            new(0x1F94C, "Curling Stone"),
            new(0x1F94D, "Lacrosse"),
            new(0x1F94E, "Softball"),
            new(0x1F94F, "Flying Disc"),
            new("\u26BD", "Soccer Ball"),
            new("\u26BE", "Baseball"),
            new("\u26F8" + emojiStyle, "Ice Skate"));

        public static readonly Emoji safetyVest = new(0x1F9BA, "Safety Vest");

        public static readonly EmojiGroup clothing = new(
            "Clothing", "Clothing",
            new(0x1F3A9, "Top Hat"),
            new(0x1F93F, "Diving Mask"),
            new(0x1F452, "Woman’s Hat"),
            new(0x1F453, "Glasses"),
            new(0x1F576 + emojiStyle, "Sunglasses"),
            new(0x1F454, "Necktie"),
            new(0x1F455, "T-Shirt"),
            new(0x1F456, "Jeans"),
            new(0x1F457, "Dress"),
            new(0x1F458, "Kimono"),
            new(0x1F459, "Bikini"),
            new(0x1F45A, "Woman’s Clothes"),
            new(0x1F45B, "Purse"),
            new(0x1F45C, "Handbag"),
            new(0x1F45D, "Clutch Bag"),
            new(0x1F45E, "Man’s Shoe"),
            new(0x1F45F, "Running Shoe"),
            new(0x1F460, "High-Heeled Shoe"),
            new(0x1F461, "Woman’s Sandal"),
            new(0x1F462, "Woman’s Boot"),
            new(0x1F94B, "Martial Arts Uniform"),
            new(0x1F97B, "Sari"),
            new(0x1F97C, "Lab Coat"),
            new(0x1F97D, "Goggles"),
            new(0x1F97E, "Hiking Boot"),
            new(0x1F97F, "Flat Shoe"),
            whiteCane,
            safetyVest,
            new(0x1F9E2, "Billed Cap"),
            new(0x1F9E3, "Scarf"),
            new(0x1F9E4, "Gloves"),
            new(0x1F9E5, "Coat"),
            new(0x1F9E6, "Socks"),
            new(0x1F9FF, "Nazar Amulet"),
            new(0x1FA70, "Ballet Shoes"),
            new(0x1FA71, "One-Piece Swimsuit"),
            new(0x1FA72, "Briefs"),
            new(0x1FA73, "Shorts"));

        public static readonly EmojiGroup town = new(
            "Town", "Town",
            new(0x1F3D7 + emojiStyle, "Building Construction"),
            new(0x1F3D8 + emojiStyle, "Houses"),
            new(0x1F3D9 + emojiStyle, "Cityscape"),
            new(0x1F3DA + emojiStyle, "Derelict House"),
            new(0x1F3DB + emojiStyle, "Classical Building"),
            new(0x1F3DC + emojiStyle, "Desert"),
            new(0x1F3DD + emojiStyle, "Desert Island"),
            new(0x1F3DE + emojiStyle, "National Park"),
            new(0x1F3DF + emojiStyle, "Stadium"),
            new(0x1F3E0, "House"),
            new(0x1F3E1, "House with Garden"),
            new(0x1F3E2, "Office Building"),
            new(0x1F3E3, "Japanese Post Office"),
            new(0x1F3E4, "Post Office"),
            new(0x1F3E5, "Hospital"),
            new(0x1F3E6, "Bank"),
            new(0x1F3E7, "ATM Sign"),
            new(0x1F3E8, "Hotel"),
            new(0x1F3E9, "Love Hotel"),
            new(0x1F3EA, "Convenience Store"),
            school,
            new(0x1F3EC, "Department Store"),
            factory,
            new(0x1F309, "Bridge at Night"),
            new("\u26F2", "Fountain"),
            new(0x1F6CD + emojiStyle, "Shopping Bags"),
            new(0x1F9FE, "Receipt"),
            new(0x1F6D2, "Shopping Cart"),
            new(0x1F488, "Barber Pole"),
            new(0x1F492, "Wedding"),
            new(0x1F5F3 + emojiStyle, "Ballot Box with Ballot"));

        public static readonly EmojiGroup music = new(
            "Music", "Music",
            new(0x1F3BC, "Musical Score"),
            new(0x1F3B6, "Musical Notes"),
            new(0x1F3B5, "Musical Note"),
            new(0x1F3B7, "Saxophone"),
            new(0x1F3B8, "Guitar"),
            new(0x1F3B9, "Musical Keyboard"),
            new(0x1F3BA, "Trumpet"),
            new(0x1F3BB, "Violin"),
            new(0x1F941, "Drum"),
            new(0x1FA97, "Accordion"),
            new(0x1FA98, "Long Drum"),
            new(0x1FA95, "Banjo"));

        public static readonly Emoji snowflake = new("\u2744" + emojiStyle, "Snowflake");
        public static readonly Emoji rainbow = new(0x1F308, "Rainbow");

        public static readonly EmojiGroup weather = new(
            "Weather", "Weather",
            new(0x1F304, "Sunrise Over Mountains"),
            new(0x1F305, "Sunrise"),
            new(0x1F306, "Cityscape at Dusk"),
            new(0x1F307, "Sunset"),
            new(0x1F303, "Night with Stars"),
            new(0x1F302, "Closed Umbrella"),
            new("\u2602" + emojiStyle, "Umbrella"),
            new("\u2614" + emojiStyle, "Umbrella with Rain Drops"),
            new("\u2603" + emojiStyle, "Snowman"),
            new("\u26C4", "Snowman Without Snow"),
            new("\u2600" + emojiStyle, "Sun"),
            new("\u2601" + emojiStyle, "Cloud"),
            new(0x1F324 + emojiStyle, "Sun Behind Small Cloud"),
            new("\u26C5", "Sun Behind Cloud"),
            new(0x1F325 + emojiStyle, "Sun Behind Large Cloud"),
            new(0x1F326 + emojiStyle, "Sun Behind Rain Cloud"),
            new(0x1F327 + emojiStyle, "Cloud with Rain"),
            new(0x1F328 + emojiStyle, "Cloud with Snow"),
            new(0x1F329 + emojiStyle, "Cloud with Lightning"),
            new("\u26C8" + emojiStyle, "Cloud with Lightning and Rain"),
            snowflake,
            new(0x1F300, "Cyclone"),
            new(0x1F32A + emojiStyle, "Tornado"),
            new(0x1F32C + emojiStyle, "Wind Face"),
            new(0x1F30A, "Water Wave"),
            new(0x1F32B + emojiStyle, "Fog"),
            new(0x1F301, "Foggy"),
            rainbow,
            new(0x1F321 + emojiStyle, "Thermometer"));

        public static readonly EmojiGroup astro = new(
            "Astronomy", "Astronomy",
            new(0x1F30C, "Milky Way"),
            new(0x1F30D, "Globe Showing Europe-Africa"),
            new(0x1F30E, "Globe Showing Americas"),
            new(0x1F30F, "Globe Showing Asia-Australia"),
            new(0x1F310, "Globe with Meridians"),
            new(0x1F311, "New Moon"),
            new(0x1F312, "Waxing Crescent Moon"),
            new(0x1F313, "First Quarter Moon"),
            new(0x1F314, "Waxing Gibbous Moon"),
            new(0x1F315, "Full Moon"),
            new(0x1F316, "Waning Gibbous Moon"),
            new(0x1F317, "Last Quarter Moon"),
            new(0x1F318, "Waning Crescent Moon"),
            new(0x1F319, "Crescent Moon"),
            new(0x1F31A, "New Moon Face"),
            new(0x1F31B, "First Quarter Moon Face"),
            new(0x1F31C, "Last Quarter Moon Face"),
            new(0x1F31D, "Full Moon Face"),
            new(0x1F31E, "Sun with Face"),
            new(0x1F31F, "Glowing Star"),
            new(0x1F320, "Shooting Star"),
            new("\u2604" + emojiStyle, "Comet"),
            new(0x1FA90, "Ringed Planet"));

        public static readonly EmojiGroup finance = new(
            "Finance", "Finance",
            new(0x1F4B0, "Money Bag"),
            new(0x1F4B1, "Currency Exchange"),
            new(0x1F4B2, "Heavy Dollar Sign"),
            new(0x1F4B3, "Credit Card"),
            new(0x1F4B4, "Yen Banknote"),
            new(0x1F4B5, "Dollar Banknote"),
            new(0x1F4B6, "Euro Banknote"),
            new(0x1F4B7, "Pound Banknote"),
            new(0x1F4B8, "Money with Wings"),
            new(0x1FA99, "Coin"),
            new(0x1F4B9, "Chart Increasing with Yen"));

        public static readonly EmojiGroup writing = new(
            "Writing", "Writing",
            new(0x1F58A + emojiStyle, "Pen"),
            new(0x1F58B + emojiStyle, "Fountain Pen"),
            new(0x1F58C + emojiStyle, "Paintbrush"),
            new(0x1F58D + emojiStyle, "Crayon"),
            new("\u270F" + emojiStyle, "Pencil"),
            new("\u2712" + emojiStyle, "Black Nib"));

        public static readonly Emoji alembic = new("\u2697" + emojiStyle, "Alembic");
        public static readonly Emoji gear = new("\u2699" + emojiStyle, "Gear");
        public static readonly Emoji atomSymbol = new("\u269B" + emojiStyle, "Atom Symbol");
        public static readonly Emoji keyboard = new("\u2328" + emojiStyle, "Keyboard");
        public static readonly Emoji telephone = new("\u260E" + emojiStyle, "Telephone");
        public static readonly Emoji studioMicrophone = new(0x1F399 + emojiStyle, "Studio Microphone");
        public static readonly Emoji levelSlider = new(0x1F39A + emojiStyle, "Level Slider");
        public static readonly Emoji controlKnobs = new(0x1F39B + emojiStyle, "Control Knobs");
        public static readonly Emoji movieCamera = new(0x1F3A5, "Movie Camera");
        public static readonly Emoji headphone = new(0x1F3A7, "Headphone");
        public static readonly Emoji videoGame = new(0x1F3AE, "Video Game");
        public static readonly Emoji lightBulb = new(0x1F4A1, "Light Bulb");
        public static readonly Emoji computerDisk = new(0x1F4BD, "Computer Disk");
        public static readonly Emoji floppyDisk = new(0x1F4BE, "Floppy Disk");
        public static readonly Emoji opticalDisk = new(0x1F4BF, "Optical Disk");
        public static readonly Emoji dvd = new(0x1F4C0, "DVD");
        public static readonly Emoji telephoneReceiver = new(0x1F4DE, "Telephone Receiver");
        public static readonly Emoji pager = new(0x1F4DF, "Pager");
        public static readonly Emoji faxMachine = new(0x1F4E0, "Fax Machine");
        public static readonly Emoji satelliteAntenna = new(0x1F4E1, "Satellite Antenna");
        public static readonly Emoji loudspeaker = new(0x1F4E2, "Loudspeaker");
        public static readonly Emoji megaphone = new(0x1F4E3, "Megaphone");
        public static readonly Emoji mobilePhone = new(0x1F4F1, "Mobile Phone");
        public static readonly Emoji mobilePhoneWithArrow = new(0x1F4F2, "Mobile Phone with Arrow");
        public static readonly Emoji mobilePhoneVibrating = new(0x1F4F3, "Mobile Phone Vibrating");
        public static readonly Emoji mobilePhoneOff = new(0x1F4F4, "Mobile Phone Off");
        public static readonly Emoji noMobilePhone = new(0x1F4F5, "No Mobile Phone");
        public static readonly Emoji antennaBars = new(0x1F4F6, "Antenna Bars");
        public static readonly Emoji camera = new(0x1F4F7, "Camera");
        public static readonly Emoji cameraWithFlash = new(0x1F4F8, "Camera with Flash");
        public static readonly Emoji videoCamera = new(0x1F4F9, "Video Camera");
        public static readonly Emoji television = new(0x1F4FA, "Television");
        public static readonly Emoji radio = new(0x1F4FB, "Radio");
        public static readonly Emoji videocassette = new(0x1F4FC, "Videocassette");
        public static readonly Emoji filmProjector = new(0x1F4FD + emojiStyle, "Film Projector");
        public static readonly Emoji portableStereo = new(0x1F4FE + emojiStyle, "Portable Stereo");
        public static readonly Emoji dimButton = new(0x1F505, "Dim Button");
        public static readonly Emoji brightButton = new(0x1F506, "Bright Button");
        public static readonly Emoji mutedSpeaker = new(0x1F507, "Muted Speaker");
        public static readonly Emoji speakerLowVolume = new(0x1F508, "Speaker Low Volume");
        public static readonly Emoji speakerMediumVolume = new(0x1F509, "Speaker Medium Volume");
        public static readonly Emoji speakerHighVolume = new(0x1F50A, "Speaker High Volume");
        public static readonly Emoji battery = new(0x1F50B, "Battery");
        public static readonly Emoji electricPlug = new(0x1F50C, "Electric Plug");
        public static readonly Emoji magnifyingGlassTiltedLeft = new(0x1F50D, "Magnifying Glass Tilted Left");
        public static readonly Emoji magnifyingGlassTiltedRight = new(0x1F50E, "Magnifying Glass Tilted Right");
        public static readonly Emoji lockedWithPen = new(0x1F50F, "Locked with Pen");
        public static readonly Emoji lockedWithKey = new(0x1F510, "Locked with Key");
        public static readonly Emoji key = new(0x1F511, "Key");
        public static readonly Emoji locked = new(0x1F512, "Locked");
        public static readonly Emoji unlocked = new(0x1F513, "Unlocked");
        public static readonly Emoji bell = new(0x1F514, "Bell");
        public static readonly Emoji bellWithSlash = new(0x1F515, "Bell with Slash");
        public static readonly Emoji bookmark = new(0x1F516, "Bookmark");
        public static readonly Emoji link = new(0x1F517, "Link");
        public static readonly Emoji joystick = new(0x1F579 + emojiStyle, "Joystick");
        public static readonly Emoji desktopComputer = new(0x1F5A5 + emojiStyle, "Desktop Computer");
        public static readonly Emoji printer = new(0x1F5A8 + emojiStyle, "Printer");
        public static readonly Emoji computerMouse = new(0x1F5B1 + emojiStyle, "Computer Mouse");
        public static readonly Emoji trackball = new(0x1F5B2 + emojiStyle, "Trackball");
        public static readonly Emoji blackFolder = new(0x1F5BF, "Black Folder");
        public static readonly Emoji folder = new(0x1F5C0, "Folder");
        public static readonly Emoji openFolder = new(0x1F5C1, "Open Folder");
        public static readonly Emoji cardIndexDividers = new(0x1F5C2, "Card Index Dividers");
        public static readonly Emoji cardFileBox = new(0x1F5C3, "Card File Box");
        public static readonly Emoji fileCabinet = new(0x1F5C4, "File Cabinet");
        public static readonly Emoji emptyNote = new(0x1F5C5, "Empty Note");
        public static readonly Emoji emptyNotePage = new(0x1F5C6, "Empty Note Page");
        public static readonly Emoji emptyNotePad = new(0x1F5C7, "Empty Note Pad");
        public static readonly Emoji note = new(0x1F5C8, "Note");
        public static readonly Emoji notePage = new(0x1F5C9, "Note Page");
        public static readonly Emoji notePad = new(0x1F5CA, "Note Pad");
        public static readonly Emoji emptyDocument = new(0x1F5CB, "Empty Document");
        public static readonly Emoji emptyPage = new(0x1F5CC, "Empty Page");
        public static readonly Emoji emptyPages = new(0x1F5CD, "Empty Pages");
        public static readonly Emoji documentIcon = new(0x1F5CE, "Document");
        public static readonly Emoji page = new(0x1F5CF, "Page");
        public static readonly Emoji pages = new(0x1F5D0, "Pages");
        public static readonly Emoji wastebasket = new(0x1F5D1, "Wastebasket");
        public static readonly Emoji spiralNotePad = new(0x1F5D2, "Spiral Note Pad");
        public static readonly Emoji spiralCalendar = new(0x1F5D3, "Spiral Calendar");
        public static readonly Emoji desktopWindow = new(0x1F5D4, "Desktop Window");
        public static readonly Emoji minimize = new(0x1F5D5, "Minimize");
        public static readonly Emoji maximize = new(0x1F5D6, "Maximize");
        public static readonly Emoji overlap = new(0x1F5D7, "Overlap");
        public static readonly Emoji reload = new(0x1F5D8, "Reload");
        public static readonly Emoji close = new(0x1F5D9, "Close");
        public static readonly Emoji increaseFontSize = new(0x1F5DA, "Increase Font Size");
        public static readonly Emoji decreaseFontSize = new(0x1F5DB, "Decrease Font Size");
        public static readonly Emoji compression = new(0x1F5DC, "Compression");
        public static readonly Emoji oldKey = new(0x1F5DD, "Old Key");
        public static readonly EmojiGroup tech = new(
            "Technology", "Technology",
            joystick,
            videoGame,
            lightBulb,
            laptop,
            briefcase,
            computerDisk,
            floppyDisk,
            opticalDisk,
            dvd,
            desktopComputer,
            keyboard,
            printer,
            computerMouse,
            trackball,
            telephone,
            telephoneReceiver,
            pager,
            faxMachine,
            satelliteAntenna,
            loudspeaker,
            megaphone,
            television,
            radio,
            videocassette,
            filmProjector,
            studioMicrophone,
            levelSlider,
            controlKnobs,
            microphone,
            movieCamera,
            headphone,
            camera,
            cameraWithFlash,
            videoCamera,
            mobilePhone,
            mobilePhoneOff,
            mobilePhoneWithArrow,
            lockedWithPen,
            lockedWithKey,
            locked,
            unlocked,
            bell,
            bellWithSlash,
            bookmark,
            link,
            mobilePhoneVibrating,
            antennaBars,
            dimButton,
            brightButton,
            mutedSpeaker,
            speakerLowVolume,
            speakerMediumVolume,
            speakerHighVolume,
            battery,
            electricPlug);

        public static readonly EmojiGroup mail = new(
            "Mail", "Mail",
            new(0x1F4E4, "Outbox Tray"),
            new(0x1F4E5, "Inbox Tray"),
            new(0x1F4E6, "Package"),
            new(0x1F4E7, "E-Mail"),
            new(0x1F4E8, "Incoming Envelope"),
            new(0x1F4E9, "Envelope with Arrow"),
            new(0x1F4EA, "Closed Mailbox with Lowered Flag"),
            new(0x1F4EB, "Closed Mailbox with Raised Flag"),
            new(0x1F4EC, "Open Mailbox with Raised Flag"),
            new(0x1F4ED, "Open Mailbox with Lowered Flag"),
            new(0x1F4EE, "Postbox"),
            new(0x1F4EF, "Postal Horn"));

        public static readonly EmojiGroup celebration = new(
            "Celebration", "Celebration",
            new(0x1F380, "Ribbon"),
            new(0x1F381, "Wrapped Gift"),
            new(0x1F383, "Jack-O-Lantern"),
            new(0x1F384, "Christmas Tree"),
            new(0x1F9E8, "Firecracker"),
            new(0x1F386, "Fireworks"),
            new(0x1F387, "Sparkler"),
            new("\u2728", "Sparkles"),
            new("\u2747" + emojiStyle, "Sparkle"),
            new(0x1F388, "Balloon"),
            new(0x1F389, "Party Popper"),
            new(0x1F38A, "Confetti Ball"),
            new(0x1F38B, "Tanabata Tree"),
            new(0x1F38D, "Pine Decoration"),
            new(0x1F38E, "Japanese Dolls"),
            new(0x1F38F, "Carp Streamer"),
            new(0x1F390, "Wind Chime"),
            new(0x1F391, "Moon Viewing Ceremony"),
            new(0x1F392, "Backpack"),
            graduationCap,
            new(0x1F9E7, "Red Envelope"),
            new(0x1F3EE, "Red Paper Lantern"),
            new(0x1F396 + emojiStyle, "Military Medal"));

        public static readonly EmojiGroup tools = new(
            "Tools", "Tools",
            new(0x1F3A3, "Fishing Pole"),
            new(0x1F526, "Flashlight"),
            wrench,
            new(0x1F528, "Hammer"),
            new(0x1F529, "Nut and Bolt"),
            new(0x1F6E0 + emojiStyle, "Hammer and Wrench"),
            new(0x1F9ED, "Compass"),
            new(0x1F9EF, "Fire Extinguisher"),
            new(0x1F9F0, "Toolbox"),
            new(0x1F9F1, "Brick"),
            new(0x1FA93, "Axe"),
            new("\u2692" + emojiStyle, "Hammer and Pick"),
            new("\u26CF" + emojiStyle, "Pick"),
            new("\u26D1" + emojiStyle, "Rescue Worker’s Helmet"),
            new("\u26D3" + emojiStyle, "Chains"),
            compression);

        public static readonly EmojiGroup office = new(
            "Office", "Office",
            new(0x1F4C1, "File Folder"),
            new(0x1F4C2, "Open File Folder"),
            new(0x1F4C3, "Page with Curl"),
            new(0x1F4C4, "Page Facing Up"),
            new(0x1F4C5, "Calendar"),
            new(0x1F4C6, "Tear-Off Calendar"),
            new(0x1F4C7, "Card Index"),
            cardIndexDividers,
            cardFileBox,
            fileCabinet,
            wastebasket,
            spiralNotePad,
            spiralCalendar,
            new(0x1F4C8, "Chart Increasing"),
            new(0x1F4C9, "Chart Decreasing"),
            new(0x1F4CA, "Bar Chart"),
            new(0x1F4CB, "Clipboard"),
            new(0x1F4CC, "Pushpin"),
            new(0x1F4CD, "Round Pushpin"),
            new(0x1F4CE, "Paperclip"),
            new(0x1F587 + emojiStyle, "Linked Paperclips"),
            new(0x1F4CF, "Straight Ruler"),
            new(0x1F4D0, "Triangular Ruler"),
            new(0x1F4D1, "Bookmark Tabs"),
            new(0x1F4D2, "Ledger"),
            new(0x1F4D3, "Notebook"),
            new(0x1F4D4, "Notebook with Decorative Cover"),
            new(0x1F4D5, "Closed Book"),
            new(0x1F4D6, "Open Book"),
            new(0x1F4D7, "Green Book"),
            new(0x1F4D8, "Blue Book"),
            new(0x1F4D9, "Orange Book"),
            new(0x1F4DA, "Books"),
            new(0x1F4DB, "Name Badge"),
            new(0x1F4DC, "Scroll"),
            new(0x1F4DD, "Memo"),
            new("\u2702" + emojiStyle, "Scissors"),
            new("\u2709" + emojiStyle, "Envelope"));

        public static readonly EmojiGroup signs = new(
            "Signs", "Signs",
            new(0x1F3A6, "Cinema"),
            noMobilePhone,
            new(0x1F51E, "No One Under Eighteen"),
            new(0x1F6AB, "Prohibited"),
            new(0x1F6AC, "Cigarette"),
            new(0x1F6AD, "No Smoking"),
            new(0x1F6AE, "Litter in Bin Sign"),
            new(0x1F6AF, "No Littering"),
            new(0x1F6B0, "Potable Water"),
            new(0x1F6B1, "Non-Potable Water"),
            new(0x1F6B3, "No Bicycles"),
            new(0x1F6B7, "No Pedestrians"),
            new(0x1F6B8, "Children Crossing"),
            new(0x1F6B9, "Men’s Room"),
            new(0x1F6BA, "Women’s Room"),
            new(0x1F6BB, "Restroom"),
            new(0x1F6BC, "Baby Symbol"),
            new(0x1F6BE, "Water Closet"),
            new(0x1F6C2, "Passport Control"),
            new(0x1F6C3, "Customs"),
            new(0x1F6C4, "Baggage Claim"),
            new(0x1F6C5, "Left Luggage"),
            new(0x1F17F + emojiStyle, "Parking Button"),
            new("\u267F", "Wheelchair Symbol"),
            new("\u2622" + emojiStyle, "Radioactive"),
            new("\u2623" + emojiStyle, "Biohazard"),
            new("\u26A0" + emojiStyle, "Warning"),
            new("\u26A1", "High Voltage"),
            new("\u26D4", "No Entry"),
            new("\u267B" + emojiStyle, "Recycling Symbol"),
            female,
            male,
            transgender);

        public static readonly EmojiGroup religion = new(
            "Religion", "Religion",
            new(0x1F52F, "Dotted Six-Pointed Star"),
            new("\u2721" + emojiStyle, "Star of David"),
            new(0x1F549 + emojiStyle, "Om"),
            new(0x1F54B, "Kaaba"),
            new(0x1F54C, "Mosque"),
            new(0x1F54D, "Synagogue"),
            new(0x1F54E, "Menorah"),
            new(0x1F6D0, "Place of Worship"),
            new(0x1F6D5, "Hindu Temple"),
            new("\u2626" + emojiStyle, "Orthodox Cross"),
            new("\u271D" + emojiStyle, "Latin Cross"),
            new("\u262A" + emojiStyle, "Star and Crescent"),
            new("\u262E" + emojiStyle, "Peace Symbol"),
            new("\u262F" + emojiStyle, "Yin Yang"),
            new("\u2638" + emojiStyle, "Wheel of Dharma"),
            new("\u267E" + emojiStyle, "Infinity"),
            new(0x1FA94, "Diya Lamp"),
            new("\u26E9" + emojiStyle, "Shinto Shrine"),
            new("\u26EA", "Church"),
            new("\u2734" + emojiStyle, "Eight-Pointed Star"),
            new(0x1F4FF, "Prayer Beads"));

        public static readonly Emoji door = new(0x1F6AA, "Door");
        public static readonly EmojiGroup household = new(
            "Household", "Household",
            new(0x1F484, "Lipstick"),
            new(0x1F48D, "Ring"),
            new(0x1F48E, "Gem Stone"),
            new(0x1F4F0, "Newspaper"),
            key,
            new(0x1F525, "Fire"),
            new(0x1F52B, "Pistol"),
            new(0x1F56F + emojiStyle, "Candle"),
            new(0x1F5BC + emojiStyle, "Framed Picture"),
            oldKey,
            new(0x1F5DE + emojiStyle, "Rolled-Up Newspaper"),
            new(0x1F5FA + emojiStyle, "World Map"),
            door,
            new(0x1F6BD, "Toilet"),
            new(0x1F6BF, "Shower"),
            new(0x1F6C1, "Bathtub"),
            new(0x1F6CB + emojiStyle, "Couch and Lamp"),
            new(0x1F6CF + emojiStyle, "Bed"),
            new(0x1F9F4, "Lotion Bottle"),
            new(0x1F9F5, "Thread"),
            new(0x1F9F6, "Yarn"),
            new(0x1F9F7, "Safety Pin"),
            new(0x1F9F8, "Teddy Bear"),
            new(0x1F9F9, "Broom"),
            new(0x1F9FA, "Basket"),
            new(0x1F9FB, "Roll of Paper"),
            new(0x1F9FC, "Soap"),
            new(0x1F9FD, "Sponge"),
            new(0x1FA91, "Chair"),
            new(0x1FA92, "Razor"),
            new(0x1F397 + emojiStyle, "Reminder Ribbon"));

        public static readonly EmojiGroup activities = new(
            "Activities", "Activities",
            new(0x1F39E + emojiStyle, "Film Frames"),
            new(0x1F39F + emojiStyle, "Admission Tickets"),
            new(0x1F3A0, "Carousel Horse"),
            new(0x1F3A1, "Ferris Wheel"),
            new(0x1F3A2, "Roller Coaster"),
            artistPalette,
            new(0x1F3AA, "Circus Tent"),
            new(0x1F3AB, "Ticket"),
            new(0x1F3AC, "Clapper Board"),
            new(0x1F3AD, "Performing Arts"));

        public static readonly EmojiGroup travel = new(
            "Travel", "Travel",
            new(0x1F3F7 + emojiStyle, "Label"),
            new(0x1F30B, "Volcano"),
            new(0x1F3D4 + emojiStyle, "Snow-Capped Mountain"),
            new("\u26F0" + emojiStyle, "Mountain"),
            new(0x1F3D5 + emojiStyle, "Camping"),
            new(0x1F3D6 + emojiStyle, "Beach with Umbrella"),
            new("\u26F1" + emojiStyle, "Umbrella on Ground"),
            new(0x1F3EF, "Japanese Castle"),
            new(0x1F463, "Footprints"),
            new(0x1F5FB, "Mount Fuji"),
            new(0x1F5FC, "Tokyo Tower"),
            new(0x1F5FD, "Statue of Liberty"),
            new(0x1F5FE, "Map of Japan"),
            new(0x1F5FF, "Moai"),
            new(0x1F6CE + emojiStyle, "Bellhop Bell"),
            new(0x1F9F3, "Luggage"),
            new("\u26F3", "Flag in Hole"),
            new("\u26FA", "Tent"),
            new("\u2668" + emojiStyle, "Hot Springs"));

        public static readonly EmojiGroup medieval = new(
            "Medieval", "Medieval",
            new(0x1F3F0, "Castle"),
            new(0x1F3F9, "Bow and Arrow"),
            crown,
            new(0x1F531, "Trident Emblem"),
            new(0x1F5E1 + emojiStyle, "Dagger"),
            new(0x1F6E1 + emojiStyle, "Shield"),
            new(0x1F52E, "Crystal Ball"),
            new("\u2694" + emojiStyle, "Crossed Swords"),
            new("\u269C" + emojiStyle, "Fleur-de-lis"));

        public static readonly Emoji doubleExclamationMark = new("\u203C" + emojiStyle, "Double Exclamation Mark");
        public static readonly Emoji interrobang = new("\u2049" + emojiStyle, "Exclamation Question Mark");
        public static readonly Emoji information = new("\u2139" + emojiStyle, "Information");
        public static readonly Emoji circledM = new("\u24C2" + emojiStyle, "Circled M");
        public static readonly Emoji checkMarkButton = new("\u2705", "Check Mark Button");
        public static readonly Emoji checkMark = new("\u2714" + emojiStyle, "Check Mark");
        public static readonly Emoji eightSpokedAsterisk = new("\u2733" + emojiStyle, "Eight-Spoked Asterisk");
        public static readonly Emoji crossMark = new("\u274C", "Cross Mark");
        public static readonly Emoji crossMarkButton = new("\u274E", "Cross Mark Button");
        public static readonly Emoji questionMark = new("\u2753", "Question Mark");
        public static readonly Emoji whiteQuestionMark = new("\u2754", "White Question Mark");
        public static readonly Emoji whiteExclamationMark = new("\u2755", "White Exclamation Mark");
        public static readonly Emoji exclamationMark = new("\u2757", "Exclamation Mark");
        public static readonly Emoji curlyLoop = new("\u27B0", "Curly Loop");
        public static readonly Emoji doubleCurlyLoop = new("\u27BF", "Double Curly Loop");
        public static readonly Emoji wavyDash = new("\u3030" + emojiStyle, "Wavy Dash");
        public static readonly Emoji partAlternationMark = new("\u303D" + emojiStyle, "Part Alternation Mark");
        public static readonly Emoji tradeMark = new("\u2122" + emojiStyle, "Trade Mark");
        public static readonly Emoji copyright = new("\u00A9" + emojiStyle, "Copyright");
        public static readonly Emoji registered = new("\u00AE" + emojiStyle, "Registered");
        public static readonly Emoji squareFourCourners = new("\u26F6" + emojiStyle, "Square: Four Corners");

        public static readonly EmojiGroup marks = new(
            "Marks", "Marks",
            doubleExclamationMark,
            interrobang,
            information,
            circledM,
            checkMarkButton,
            checkMark,
            eightSpokedAsterisk,
            crossMark,
            crossMarkButton,
            questionMark,
            whiteQuestionMark,
            whiteExclamationMark,
            exclamationMark,
            curlyLoop,
            doubleCurlyLoop,
            wavyDash,
            partAlternationMark,
            tradeMark,
            copyright,
            registered);

        public static readonly Emoji droplet = new(0x1F4A7, "Droplet");
        public static readonly Emoji dropOfBlood = new(0x1FA78, "Drop of Blood");
        public static readonly Emoji adhesiveBandage = new(0x1FA79, "Adhesive Bandage");
        public static readonly Emoji stethoscope = new(0x1FA7A, "Stethoscope");
        public static readonly Emoji syringe = new(0x1F489, "Syringe");
        public static readonly Emoji pill = new(0x1F48A, "Pill");
        public static readonly Emoji testTube = new(0x1F9EA, "Test Tube");
        public static readonly Emoji petriDish = new(0x1F9EB, "Petri Dish");
        public static readonly Emoji dna = new(0x1F9EC, "DNA");
        public static readonly Emoji abacus = new(0x1F9EE, "Abacus");
        public static readonly Emoji magnet = new(0x1F9F2, "Magnet");
        public static readonly Emoji telescope = new(0x1F52D, "Telescope");

        public static readonly EmojiGroup science = new(
            "Science", "Science",
            droplet,
            dropOfBlood,
            adhesiveBandage,
            stethoscope,
            syringe,
            pill,
            microscope,
            testTube,
            petriDish,
            dna,
            abacus,
            magnet,
            telescope,
            medical,
            balanceScale,
            alembic,
            gear,
            atomSymbol,
            magnifyingGlassTiltedLeft,
            magnifyingGlassTiltedRight);

        public static readonly Emoji whiteChessKing = new("\u2654", "White Chess King");
        public static readonly Emoji whiteChessQueen = new("\u2655", "White Chess Queen");
        public static readonly Emoji whiteChessRook = new("\u2656", "White Chess Rook");
        public static readonly Emoji whiteChessBishop = new("\u2657", "White Chess Bishop");
        public static readonly Emoji whiteChessKnight = new("\u2658", "White Chess Knight");
        public static readonly Emoji whiteChessPawn = new("\u2659", "White Chess Pawn");
        public static readonly EmojiGroup whiteChessPieces = new(
            whiteChessKing.Value + whiteChessQueen + whiteChessRook + whiteChessBishop + whiteChessKnight + whiteChessPawn,
            "White Chess Pieces",
            whiteChessKing,
            whiteChessQueen,
            whiteChessRook,
            whiteChessBishop,
            whiteChessKnight,
            whiteChessPawn);

        public static readonly Emoji blackChessKing = new("\u265A", "Black Chess King");
        public static readonly Emoji blackChessQueen = new("\u265B", "Black Chess Queen");
        public static readonly Emoji blackChessRook = new("\u265C", "Black Chess Rook");
        public static readonly Emoji blackChessBishop = new("\u265D", "Black Chess Bishop");
        public static readonly Emoji blackChessKnight = new("\u265E", "Black Chess Knight");
        public static readonly Emoji blackChessPawn = new("\u265F", "Black Chess Pawn");
        public static readonly EmojiGroup blackChessPieces = new(
            blackChessKing.Value + blackChessQueen + blackChessRook + blackChessBishop + blackChessKnight + blackChessPawn,
            "Black Chess Pieces",
            blackChessKing,
            blackChessQueen,
            blackChessRook,
            blackChessBishop,
            blackChessKnight,
            blackChessPawn);

        public static readonly EmojiGroup chessPawns = new(
            whiteChessPawn.Value + blackChessPawn,
            "Chess Pawns",
            whiteChessPawn,
            blackChessPawn);

        public static readonly EmojiGroup chessRooks = new(
            whiteChessRook.Value + blackChessRook,
            "Chess Rooks",
            whiteChessRook,
            blackChessRook);

        public static readonly EmojiGroup chessBishops = new(
            whiteChessBishop.Value + blackChessBishop,
            "Chess Bishops",
            whiteChessBishop,
            blackChessBishop);

        public static readonly EmojiGroup chessKnights = new(
            whiteChessKnight.Value + blackChessKnight,
            "Chess Knights",
            whiteChessKnight,
            blackChessKnight);

        public static readonly EmojiGroup chessQueens = new(
            whiteChessQueen.Value + blackChessQueen,
            "Chess Queens",
            whiteChessQueen,
            blackChessQueen);

        public static readonly EmojiGroup chessKings = new(
            whiteChessKing.Value + blackChessKing,
            "Chess Kings",
            whiteChessKing,
            blackChessKing);

        public static readonly EmojiGroup chess = new(
            "Chess Pieces",
            "Chess Pieces",
            whiteChessPieces,
            blackChessPieces,
            chessPawns,
            chessRooks,
            chessBishops,
            chessKnights,
            chessQueens,
            chessKings);

        public static readonly Emoji dice1 = new("\u2680", "Dice: Side 1");
        public static readonly Emoji dice2 = new("\u2681", "Dice: Side 2");
        public static readonly Emoji dice3 = new("\u2682", "Dice: Side 3");
        public static readonly Emoji dice4 = new("\u2683", "Dice: Side 4");
        public static readonly Emoji dice5 = new("\u2684", "Dice: Side 5");
        public static readonly Emoji dice6 = new("\u2685", "Dice: Side 6");
        public static readonly EmojiGroup dice = new(
            "Dice",
            "Dice",
            dice1,
            dice2,
            dice3,
            dice4,
            dice5,
            dice6);

        public static readonly Emoji crossedFlags = new(0x1F38C, "Crossed Flags");
        public static readonly Emoji chequeredFlag = new(0x1F3C1, "Chequered Flag");
        public static readonly Emoji whiteFlag = new(0x1F3F3 + emojiStyle, "White Flag");
        public static readonly Emoji blackFlag = new(0x1F3F4, "Black Flag");
        public static readonly Emoji rainbowFlag = whiteFlag.Join(rainbow, "Rainbow Flag");
        public static readonly Emoji transgenderFlag = whiteFlag.Join(transgender, "Transgender Flag");
        public static readonly Emoji pirateFlag = blackFlag.Join(skullAndCrossbones, "Pirate Flag");
        public static readonly Emoji flagEngland = Combo("Flag: England",
            blackFlag,
            tagLatinSmallLetterG,
            tagLatinSmallLetterB,
            tagLatinSmallLetterE,
            tagLatinSmallLetterN,
            tagLatinSmallLetterG,
            cancelTag);
        public static readonly Emoji flagScotland = Combo("Flag: Scotland",
            blackFlag,
            tagLatinSmallLetterG,
            tagLatinSmallLetterB,
            tagLatinSmallLetterS,
            tagLatinSmallLetterC,
            tagLatinSmallLetterT,
            cancelTag);
        public static readonly Emoji flagWales = Combo("Flag: Wales",
            blackFlag,
            tagLatinSmallLetterG,
            tagLatinSmallLetterB,
            tagLatinSmallLetterW,
            tagLatinSmallLetterL,
            tagLatinSmallLetterS,
            cancelTag);
        public static readonly Emoji triangularFlag = new(0x1F6A9, "Triangular Flag");

        public static readonly EmojiGroup flags = new(
            "Flags", "Basic flags",
            crossedFlags,
            chequeredFlag,
            whiteFlag,
            rainbowFlag,
            transgenderFlag,
            blackFlag,
            pirateFlag,
            flagEngland,
            flagScotland,
            flagWales,
            triangularFlag);

        public static readonly Emoji cat = new(0x1F408, "Cat");
        public static readonly Emoji dog = new(0x1F415, "Dog");
        public static readonly Emoji bear = new(0x1F43B, "Bear");
        public static readonly Emoji blackCat = cat.Join(blackLargeSquare, "Black Cat");
        public static readonly Emoji serviceDog = dog.Join(safetyVest, "Service Dog");
        public static readonly Emoji polarBear = bear.Join(snowflake, "Polar Bear");

        public static readonly EmojiGroup animals = new(
            "Animals", "Animals and insects",
            new(0x1F400, "Rat"),
            new(0x1F401, "Mouse"),
            new(0x1F402, "Ox"),
            new(0x1F403, "Water Buffalo"),
            new(0x1F404, "Cow"),
            new(0x1F405, "Tiger"),
            new(0x1F406, "Leopard"),
            new(0x1F407, "Rabbit"),
            cat,
            blackCat,
            new(0x1F409, "Dragon"),
            new(0x1F40A, "Crocodile"),
            new(0x1F40B, "Whale"),
            new(0x1F40C, "Snail"),
            new(0x1F40D, "Snake"),
            new(0x1F40E, "Horse"),
            new(0x1F40F, "Ram"),
            new(0x1F410, "Goat"),
            new(0x1F411, "Ewe"),
            new(0x1F412, "Monkey"),
            new(0x1F413, "Rooster"),
            new(0x1F414, "Chicken"),
            dog,
            serviceDog,
            new(0x1F416, "Pig"),
            new(0x1F417, "Boar"),
            new(0x1F418, "Elephant"),
            new(0x1F419, "Octopus"),
            new(0x1F41A, "Spiral Shell"),
            new(0x1F41B, "Bug"),
            new(0x1F41C, "Ant"),
            new(0x1F41D, "Honeybee"),
            new(0x1F41E, "Lady Beetle"),
            new(0x1F41F, "Fish"),
            new(0x1F420, "Tropical Fish"),
            new(0x1F421, "Blowfish"),
            new(0x1F422, "Turtle"),
            new(0x1F423, "Hatching Chick"),
            new(0x1F424, "Baby Chick"),
            new(0x1F425, "Front-Facing Baby Chick"),
            new(0x1F426, "Bird"),
            new(0x1F427, "Penguin"),
            new(0x1F428, "Koala"),
            new(0x1F429, "Poodle"),
            new(0x1F42A, "Camel"),
            new(0x1F42B, "Two-Hump Camel"),
            new(0x1F42C, "Dolphin"),
            new(0x1F42D, "Mouse Face"),
            new(0x1F42E, "Cow Face"),
            new(0x1F42F, "Tiger Face"),
            new(0x1F430, "Rabbit Face"),
            new(0x1F431, "Cat Face"),
            new(0x1F432, "Dragon Face"),
            new(0x1F433, "Spouting Whale"),
            new(0x1F434, "Horse Face"),
            new(0x1F435, "Monkey Face"),
            new(0x1F436, "Dog Face"),
            new(0x1F437, "Pig Face"),
            new(0x1F438, "Frog"),
            new(0x1F439, "Hamster"),
            new(0x1F43A, "Wolf"),
            bear,
            polarBear,
            new(0x1F43C, "Panda"),
            new(0x1F43D, "Pig Nose"),
            new(0x1F43E, "Paw Prints"),
            new(0x1F43F + emojiStyle, "Chipmunk"),
            new(0x1F54A + emojiStyle, "Dove"),
            new(0x1F577 + emojiStyle, "Spider"),
            new(0x1F578 + emojiStyle, "Spider Web"),
            new(0x1F981, "Lion"),
            new(0x1F982, "Scorpion"),
            new(0x1F983, "Turkey"),
            new(0x1F984, "Unicorn"),
            new(0x1F985, "Eagle"),
            new(0x1F986, "Duck"),
            new(0x1F987, "Bat"),
            new(0x1F988, "Shark"),
            new(0x1F989, "Owl"),
            new(0x1F98A, "Fox"),
            new(0x1F98B, "Butterfly"),
            new(0x1F98C, "Deer"),
            new(0x1F98D, "Gorilla"),
            new(0x1F98E, "Lizard"),
            new(0x1F98F, "Rhinoceros"),
            new(0x1F992, "Giraffe"),
            new(0x1F993, "Zebra"),
            new(0x1F994, "Hedgehog"),
            new(0x1F995, "Sauropod"),
            new(0x1F996, "T-Rex"),
            new(0x1F997, "Cricket"),
            new(0x1F998, "Kangaroo"),
            new(0x1F999, "Llama"),
            new(0x1F99A, "Peacock"),
            new(0x1F99B, "Hippopotamus"),
            new(0x1F99C, "Parrot"),
            new(0x1F99D, "Raccoon"),
            new(0x1F99F, "Mosquito"),
            new(0x1F9A0, "Microbe"),
            new(0x1F9A1, "Badger"),
            new(0x1F9A2, "Swan"),
            new(0x1F9A3, "Mammoth"),
            new(0x1F9A4, "Dodo"),
            new(0x1F9A5, "Sloth"),
            new(0x1F9A6, "Otter"),
            new(0x1F9A7, "Orangutan"),
            new(0x1F9A8, "Skunk"),
            new(0x1F9A9, "Flamingo"),
            new(0x1F9AB, "Beaver"),
            new(0x1F9AC, "Bison"),
            new(0x1F9AD, "Seal"),
            new(0x1FAB0, "Fly"),
            new(0x1FAB1, "Worm"),
            new(0x1FAB2, "Beetle"),
            new(0x1FAB3, "Cockroach"),
            new(0x1FAB6, "Feather"),
            new(0x1F9AE, "Guide Dog"));

        public static readonly EmojiGroup nations = new(
            "National Flags", "Flags of countries from around the world",
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterC, "Flag: Ascension Island"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterD, "Flag: Andorra"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterE, "Flag: United Arab Emirates"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterF, "Flag: Afghanistan"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterG, "Flag: Antigua & Barbuda"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterI, "Flag: Anguilla"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterL, "Flag: Albania"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterM, "Flag: Armenia"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterO, "Flag: Angola"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterQ, "Flag: Antarctica"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterR, "Flag: Argentina"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterS, "Flag: American Samoa"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterT, "Flag: Austria"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterU, "Flag: Australia"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterW, "Flag: Aruba"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterX, "Flag: Åland Islands"),
            new(regionalIndicatorSymbolLetterA + regionalIndicatorSymbolLetterZ, "Flag: Azerbaijan"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterA, "Flag: Bosnia & Herzegovina"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterB, "Flag: Barbados"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterD, "Flag: Bangladesh"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterE, "Flag: Belgium"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterF, "Flag: Burkina Faso"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterG, "Flag: Bulgaria"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterH, "Flag: Bahrain"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterI, "Flag: Burundi"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterJ, "Flag: Benin"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterL, "Flag: St. Barthélemy"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterM, "Flag: Bermuda"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterN, "Flag: Brunei"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterO, "Flag: Bolivia"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterQ, "Flag: Caribbean Netherlands"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterR, "Flag: Brazil"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterS, "Flag: Bahamas"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterT, "Flag: Bhutan"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterV, "Flag: Bouvet Island"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterW, "Flag: Botswana"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterY, "Flag: Belarus"),
            new(regionalIndicatorSymbolLetterB + regionalIndicatorSymbolLetterZ, "Flag: Belize"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterA, "Flag: Canada"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterC, "Flag: Cocos (Keeling) Islands"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterD, "Flag: Congo - Kinshasa"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterF, "Flag: Central African Republic"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterG, "Flag: Congo - Brazzaville"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterH, "Flag: Switzerland"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterI, "Flag: Côte d’Ivoire"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterK, "Flag: Cook Islands"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterL, "Flag: Chile"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterM, "Flag: Cameroon"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterN, "Flag: China"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterO, "Flag: Colombia"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterP, "Flag: Clipperton Island"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterR, "Flag: Costa Rica"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterU, "Flag: Cuba"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterV, "Flag: Cape Verde"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterW, "Flag: Curaçao"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterX, "Flag: Christmas Island"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterY, "Flag: Cyprus"),
            new(regionalIndicatorSymbolLetterC + regionalIndicatorSymbolLetterZ, "Flag: Czechia"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterE, "Flag: Germany"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterG, "Flag: Diego Garcia"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterJ, "Flag: Djibouti"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterK, "Flag: Denmark"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterM, "Flag: Dominica"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterO, "Flag: Dominican Republic"),
            new(regionalIndicatorSymbolLetterD + regionalIndicatorSymbolLetterZ, "Flag: Algeria"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterA, "Flag: Ceuta & Melilla"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterC, "Flag: Ecuador"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterE, "Flag: Estonia"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterG, "Flag: Egypt"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterH, "Flag: Western Sahara"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterR, "Flag: Eritrea"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterS, "Flag: Spain"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterT, "Flag: Ethiopia"),
            new(regionalIndicatorSymbolLetterE + regionalIndicatorSymbolLetterU, "Flag: European Union"),
            new(regionalIndicatorSymbolLetterF + regionalIndicatorSymbolLetterI, "Flag: Finland"),
            new(regionalIndicatorSymbolLetterF + regionalIndicatorSymbolLetterJ, "Flag: Fiji"),
            new(regionalIndicatorSymbolLetterF + regionalIndicatorSymbolLetterK, "Flag: Falkland Islands"),
            new(regionalIndicatorSymbolLetterF + regionalIndicatorSymbolLetterM, "Flag: Micronesia"),
            new(regionalIndicatorSymbolLetterF + regionalIndicatorSymbolLetterO, "Flag: Faroe Islands"),
            new(regionalIndicatorSymbolLetterF + regionalIndicatorSymbolLetterR, "Flag: France"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterA, "Flag: Gabon"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterB, "Flag: United Kingdom"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterD, "Flag: Grenada"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterE, "Flag: Georgia"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterF, "Flag: French Guiana"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterG, "Flag: Guernsey"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterH, "Flag: Ghana"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterI, "Flag: Gibraltar"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterL, "Flag: Greenland"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterM, "Flag: Gambia"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterN, "Flag: Guinea"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterP, "Flag: Guadeloupe"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterQ, "Flag: Equatorial Guinea"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterR, "Flag: Greece"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterS, "Flag: South Georgia & South Sandwich Islands"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterT, "Flag: Guatemala"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterU, "Flag: Guam"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterW, "Flag: Guinea-Bissau"),
            new(regionalIndicatorSymbolLetterG + regionalIndicatorSymbolLetterY, "Flag: Guyana"),
            new(regionalIndicatorSymbolLetterH + regionalIndicatorSymbolLetterK, "Flag: Hong Kong SAR China"),
            new(regionalIndicatorSymbolLetterH + regionalIndicatorSymbolLetterM, "Flag: Heard & McDonald Islands"),
            new(regionalIndicatorSymbolLetterH + regionalIndicatorSymbolLetterN, "Flag: Honduras"),
            new(regionalIndicatorSymbolLetterH + regionalIndicatorSymbolLetterR, "Flag: Croatia"),
            new(regionalIndicatorSymbolLetterH + regionalIndicatorSymbolLetterT, "Flag: Haiti"),
            new(regionalIndicatorSymbolLetterH + regionalIndicatorSymbolLetterU, "Flag: Hungary"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterC, "Flag: Canary Islands"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterD, "Flag: Indonesia"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterE, "Flag: Ireland"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterL, "Flag: Israel"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterM, "Flag: Isle of Man"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterN, "Flag: India"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterO, "Flag: British Indian Ocean Territory"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterQ, "Flag: Iraq"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterR, "Flag: Iran"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterS, "Flag: Iceland"),
            new(regionalIndicatorSymbolLetterI + regionalIndicatorSymbolLetterT, "Flag: Italy"),
            new(regionalIndicatorSymbolLetterJ + regionalIndicatorSymbolLetterE, "Flag: Jersey"),
            new(regionalIndicatorSymbolLetterJ + regionalIndicatorSymbolLetterM, "Flag: Jamaica"),
            new(regionalIndicatorSymbolLetterJ + regionalIndicatorSymbolLetterO, "Flag: Jordan"),
            new(regionalIndicatorSymbolLetterJ + regionalIndicatorSymbolLetterP, "Flag: Japan"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterE, "Flag: Kenya"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterG, "Flag: Kyrgyzstan"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterH, "Flag: Cambodia"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterI, "Flag: Kiribati"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterM, "Flag: Comoros"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterN, "Flag: St. Kitts & Nevis"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterP, "Flag: North Korea"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterR, "Flag: South Korea"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterW, "Flag: Kuwait"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterY, "Flag: Cayman Islands"),
            new(regionalIndicatorSymbolLetterK + regionalIndicatorSymbolLetterZ, "Flag: Kazakhstan"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterA, "Flag: Laos"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterB, "Flag: Lebanon"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterC, "Flag: St. Lucia"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterI, "Flag: Liechtenstein"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterK, "Flag: Sri Lanka"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterR, "Flag: Liberia"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterS, "Flag: Lesotho"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterT, "Flag: Lithuania"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterU, "Flag: Luxembourg"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterV, "Flag: Latvia"),
            new(regionalIndicatorSymbolLetterL + regionalIndicatorSymbolLetterY, "Flag: Libya"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterA, "Flag: Morocco"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterC, "Flag: Monaco"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterD, "Flag: Moldova"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterE, "Flag: Montenegro"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterF, "Flag: St. Martin"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterG, "Flag: Madagascar"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterH, "Flag: Marshall Islands"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterK, "Flag: North Macedonia"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterL, "Flag: Mali"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterM, "Flag: Myanmar (Burma)"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterN, "Flag: Mongolia"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterO, "Flag: Macao Sar China"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterP, "Flag: Northern Mariana Islands"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterQ, "Flag: Martinique"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterR, "Flag: Mauritania"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterS, "Flag: Montserrat"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterT, "Flag: Malta"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterU, "Flag: Mauritius"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterV, "Flag: Maldives"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterW, "Flag: Malawi"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterX, "Flag: Mexico"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterY, "Flag: Malaysia"),
            new(regionalIndicatorSymbolLetterM + regionalIndicatorSymbolLetterZ, "Flag: Mozambique"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterA, "Flag: Namibia"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterC, "Flag: New Caledonia"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterE, "Flag: Niger"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterF, "Flag: Norfolk Island"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterG, "Flag: Nigeria"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterI, "Flag: Nicaragua"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterL, "Flag: Netherlands"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterO, "Flag: Norway"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterP, "Flag: Nepal"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterR, "Flag: Nauru"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterU, "Flag: Niue"),
            new(regionalIndicatorSymbolLetterN + regionalIndicatorSymbolLetterZ, "Flag: New Zealand"),
            new(regionalIndicatorSymbolLetterO + regionalIndicatorSymbolLetterM, "Flag: Oman"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterA, "Flag: Panama"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterE, "Flag: Peru"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterF, "Flag: French Polynesia"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterG, "Flag: Papua New Guinea"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterH, "Flag: Philippines"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterK, "Flag: Pakistan"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterL, "Flag: Poland"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterM, "Flag: St. Pierre & Miquelon"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterN, "Flag: Pitcairn Islands"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterR, "Flag: Puerto Rico"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterS, "Flag: Palestinian Territories"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterT, "Flag: Portugal"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterW, "Flag: Palau"),
            new(regionalIndicatorSymbolLetterP + regionalIndicatorSymbolLetterY, "Flag: Paraguay"),
            new(regionalIndicatorSymbolLetterQ + regionalIndicatorSymbolLetterA, "Flag: Qatar"),
            new(regionalIndicatorSymbolLetterR + regionalIndicatorSymbolLetterE, "Flag: Réunion"),
            new(regionalIndicatorSymbolLetterR + regionalIndicatorSymbolLetterO, "Flag: Romania"),
            new(regionalIndicatorSymbolLetterR + regionalIndicatorSymbolLetterS, "Flag: Serbia"),
            new(regionalIndicatorSymbolLetterR + regionalIndicatorSymbolLetterU, "Flag: Russia"),
            new(regionalIndicatorSymbolLetterR + regionalIndicatorSymbolLetterW, "Flag: Rwanda"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterA, "Flag: Saudi Arabia"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterB, "Flag: Solomon Islands"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterC, "Flag: Seychelles"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterD, "Flag: Sudan"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterE, "Flag: Sweden"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterG, "Flag: Singapore"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterH, "Flag: St. Helena"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterI, "Flag: Slovenia"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterJ, "Flag: Svalbard & Jan Mayen"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterK, "Flag: Slovakia"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterL, "Flag: Sierra Leone"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterM, "Flag: San Marino"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterN, "Flag: Senegal"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterO, "Flag: Somalia"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterR, "Flag: Suriname"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterS, "Flag: South Sudan"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterT, "Flag: São Tomé & Príncipe"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterV, "Flag: El Salvador"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterX, "Flag: Sint Maarten"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterY, "Flag: Syria"),
            new(regionalIndicatorSymbolLetterS + regionalIndicatorSymbolLetterZ, "Flag: Eswatini"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterA, "Flag: Tristan Da Cunha"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterC, "Flag: Turks & Caicos Islands"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterD, "Flag: Chad"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterF, "Flag: French Southern Territories"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterG, "Flag: Togo"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterH, "Flag: Thailand"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterJ, "Flag: Tajikistan"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterK, "Flag: Tokelau"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterL, "Flag: Timor-Leste"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterM, "Flag: Turkmenistan"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterN, "Flag: Tunisia"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterO, "Flag: Tonga"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterR, "Flag: Turkey"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterT, "Flag: Trinidad & Tobago"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterV, "Flag: Tuvalu"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterW, "Flag: Taiwan"),
            new(regionalIndicatorSymbolLetterT + regionalIndicatorSymbolLetterZ, "Flag: Tanzania"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterA, "Flag: Ukraine"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterG, "Flag: Uganda"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterM, "Flag: U.S. Outlying Islands"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterN, "Flag: United Nations"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterS, "Flag: United States"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterY, "Flag: Uruguay"),
            new(regionalIndicatorSymbolLetterU + regionalIndicatorSymbolLetterZ, "Flag: Uzbekistan"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterA, "Flag: Vatican City"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterC, "Flag: St. Vincent & Grenadines"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterE, "Flag: Venezuela"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterG, "Flag: British Virgin Islands"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterI, "Flag: U.S. Virgin Islands"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterN, "Flag: Vietnam"),
            new(regionalIndicatorSymbolLetterV + regionalIndicatorSymbolLetterU, "Flag: Vanuatu"),
            new(regionalIndicatorSymbolLetterW + regionalIndicatorSymbolLetterF, "Flag: Wallis & Futuna"),
            new(regionalIndicatorSymbolLetterW + regionalIndicatorSymbolLetterS, "Flag: Samoa"),
            new(regionalIndicatorSymbolLetterX + regionalIndicatorSymbolLetterK, "Flag: Kosovo"),
            new(regionalIndicatorSymbolLetterY + regionalIndicatorSymbolLetterE, "Flag: Yemen"),
            new(regionalIndicatorSymbolLetterY + regionalIndicatorSymbolLetterT, "Flag: Mayotte"),
            new(regionalIndicatorSymbolLetterZ + regionalIndicatorSymbolLetterA, "Flag: South Africa"),
            new(regionalIndicatorSymbolLetterZ + regionalIndicatorSymbolLetterM, "Flag: Zambia"),
            new(regionalIndicatorSymbolLetterZ + regionalIndicatorSymbolLetterW, "Flag: Zimbabwe"));

        public static readonly EmojiGroup allIcons = new(
            "All Icons",
            "All Icons",
            faces,
            love,
            cartoon,
            hands,
            bodyParts,
            people,
            gestures,
            inMotion,
            resting,
            roles,
            fantasy,
            animals,
            plants,
            food,
            flags,
            vehicles,
            clocks,
            arrows,
            shapes,
            buttons,
            zodiac,
            chess,
            dice,
            math,
            games,
            sportsEquipment,
            clothing,
            town,
            music,
            weather,
            astro,
            finance,
            writing,
            science,
            tech,
            mail,
            celebration,
            tools,
            office,
            signs,
            religion,
            household,
            activities,
            travel,
            medieval);
    }
}
