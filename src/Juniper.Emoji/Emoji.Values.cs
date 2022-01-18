using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class Emoji
    {
        public static readonly Emoji textStyle = new("\uFE0E", "Variation Selector-15: text style");
        public static readonly Emoji emojiStyle = new("\uFE0F", "Variation Selector-16: emoji style");
        public static readonly Emoji zeroWidthJoiner = new("\u200D", "Zero Width Joiner");
        public static readonly Emoji combiningEnclosingKeycap = new("\u20E3", "Combining Enclosing Keycap");
        public static readonly IReadOnlyList<Emoji> combiners = new[] {
            textStyle,
            emojiStyle,
            zeroWidthJoiner,
            combiningEnclosingKeycap,
        };

        public static readonly Emoji female = new("\u2640" + emojiStyle.Value, "Female");
        public static readonly Emoji male = new("\u2642" + emojiStyle.Value, "Male");
        public static readonly Emoji transgender = new("\u26A7" + emojiStyle.Value, "Transgender Symbol");
        public static readonly IReadOnlyList<Emoji> sexes = new[] {
            female,
            male,
        };
        public static readonly Emoji skinL = new(char.ConvertFromUtf32(0x1F3FB), "Light Skin Tone");
        public static readonly Emoji skinML = new(char.ConvertFromUtf32(0x1F3FC), "Medium-Light Skin Tone");
        public static readonly Emoji skinM = new(char.ConvertFromUtf32(0x1F3FD), "Medium Skin Tone");
        public static readonly Emoji skinMD = new(char.ConvertFromUtf32(0x1F3FE), "Medium-Dark Skin Tone");
        public static readonly Emoji skinD = new(char.ConvertFromUtf32(0x1F3FF), "Dark Skin Tone");
        public static readonly IReadOnlyList<Emoji> skinTones = new[] {
            skinL,
            skinML,
            skinM,
            skinMD,
            skinD,
        };
        public static readonly Emoji hairRed = new(char.ConvertFromUtf32(0x1F9B0), "Red Hair");
        public static readonly Emoji hairCurly = new(char.ConvertFromUtf32(0x1F9B1), "Curly Hair");
        public static readonly Emoji hairWhite = new(char.ConvertFromUtf32(0x1F9B3), "White Hair");
        public static readonly Emoji hairBald = new(char.ConvertFromUtf32(0x1F9B2), "Bald");
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

        private static Emoji Combo(Emoji a, Emoji b, string altDesc = null)
        {
            return new(a.Value + b.Value, altDesc ?? a.Desc + b.Desc);
        }

        private static Emoji Join(Emoji a, Emoji b, string altDesc = null)
        {
            return new(a.Value + zeroWidthJoiner.Value + b.Value, altDesc ?? $"{a.Desc}: {b.Desc}");
        }

        private static EmojiGroup Join(EmojiGroup A, Emoji b)
        {
            var temp = Join(A as Emoji, b);
            var alts = A.Alts.Select(a => Join(a, b)).ToArray();
            return new(temp, alts);
        }

        private static EmojiGroup Skin(string v, string d, params Emoji[] rest)
        {
            var person = new Emoji(v, d);
            var light = Combo(person, skinL);
            var mediumLight = Combo(person, skinML);
            var medium = Combo(person, skinM);
            var mediumDark = Combo(person, skinMD);
            var dark = Combo(person, skinD);
            var alts = new[]
            {
                person,
                light,
                mediumLight,
                medium,
                mediumDark,
                dark
            }.Concat(rest)
            .ToArray();
            return new(v, d, alts);
        }

        private static EmojiGroup Sex(Emoji person)
        {
            var man = Join(person, male);
            var woman = Join(person, female);

            return new(person, person, man, woman);
        }

        private static EmojiGroup SkinAndSex(string v, string d)
        {
            return Sex(Skin(v, d));
        }

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
            var j = new Emoji(symbol.Value, name);
            var men = Join(man.Alts[0], j);
            var women = Join(woman.Alts[0], j);
            return new(symbol, symbol, men, women);
        }

        public static readonly EmojiGroup frowners = SkinAndSex(char.ConvertFromUtf32(0x1F64D), "Frowning");
        public static readonly EmojiGroup pouters = SkinAndSex(char.ConvertFromUtf32(0x1F64E), "Pouting");
        public static readonly EmojiGroup gesturingNo = SkinAndSex(char.ConvertFromUtf32(0x1F645), "Gesturing NO");
        public static readonly EmojiGroup gesturingOK = SkinAndSex(char.ConvertFromUtf32(0x1F646), "Gesturing OK");
        public static readonly EmojiGroup tippingHand = SkinAndSex(char.ConvertFromUtf32(0x1F481), "Tipping Hand");
        public static readonly EmojiGroup raisingHand = SkinAndSex(char.ConvertFromUtf32(0x1F64B), "Raising Hand");
        public static readonly EmojiGroup bowing = SkinAndSex(char.ConvertFromUtf32(0x1F647), "Bowing");
        public static readonly EmojiGroup facePalming = SkinAndSex(char.ConvertFromUtf32(0x1F926), "Facepalming");
        public static readonly EmojiGroup shrugging = SkinAndSex(char.ConvertFromUtf32(0x1F937), "Shrugging");
        public static readonly EmojiGroup cantHear = SkinAndSex(char.ConvertFromUtf32(0x1F9CF), "Can't Hear");
        public static readonly EmojiGroup gettingMassage = SkinAndSex(char.ConvertFromUtf32(0x1F486), "Getting Massage");
        public static readonly EmojiGroup gettingHaircut = SkinAndSex(char.ConvertFromUtf32(0x1F487), "Getting Haircut");

        public static readonly EmojiGroup constructionWorkers = SkinAndSex(char.ConvertFromUtf32(0x1F477), "Construction Worker");
        public static readonly EmojiGroup guards = SkinAndSex(char.ConvertFromUtf32(0x1F482), "Guard");
        public static readonly EmojiGroup spies = SkinAndSex(char.ConvertFromUtf32(0x1F575), "Spy");
        public static readonly EmojiGroup police = SkinAndSex(char.ConvertFromUtf32(0x1F46E), "Police");
        public static readonly EmojiGroup wearingTurban = SkinAndSex(char.ConvertFromUtf32(0x1F473), "Wearing Turban");
        public static readonly EmojiGroup superheroes = SkinAndSex(char.ConvertFromUtf32(0x1F9B8), "Superhero");
        public static readonly EmojiGroup supervillains = SkinAndSex(char.ConvertFromUtf32(0x1F9B9), "Supervillain");
        public static readonly EmojiGroup mages = SkinAndSex(char.ConvertFromUtf32(0x1F9D9), "Mage");
        public static readonly EmojiGroup fairies = SkinAndSex(char.ConvertFromUtf32(0x1F9DA), "Fairy");
        public static readonly EmojiGroup vampires = SkinAndSex(char.ConvertFromUtf32(0x1F9DB), "Vampire");
        public static readonly EmojiGroup merpeople = SkinAndSex(char.ConvertFromUtf32(0x1F9DC), "Merperson");
        public static readonly EmojiGroup elves = SkinAndSex(char.ConvertFromUtf32(0x1F9DD), "Elf");
        public static readonly EmojiGroup walking = SkinAndSex(char.ConvertFromUtf32(0x1F6B6), "Walking");
        public static readonly EmojiGroup standing = SkinAndSex(char.ConvertFromUtf32(0x1F9CD), "Standing");
        public static readonly EmojiGroup kneeling = SkinAndSex(char.ConvertFromUtf32(0x1F9CE), "Kneeling");
        public static readonly EmojiGroup runners = SkinAndSex(char.ConvertFromUtf32(0x1F3C3), "Running");

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


        public static readonly EmojiGroup baby = Skin(char.ConvertFromUtf32(0x1F476), "Baby");
        public static readonly EmojiGroup child = Skin(char.ConvertFromUtf32(0x1F9D2), "Child");
        public static readonly EmojiGroup boy = Skin(char.ConvertFromUtf32(0x1F466), "Boy");
        public static readonly EmojiGroup girl = Skin(char.ConvertFromUtf32(0x1F467), "Girl");
        public static readonly EmojiGroup children = new(child, child, boy, girl);


        public static readonly EmojiGroup blondes = SkinAndSex(char.ConvertFromUtf32(0x1F471), "Blond Person");
        public static readonly EmojiGroup person = Skin(char.ConvertFromUtf32(0x1F9D1), "Person",
            blondes,
            wearingTurban);

        public static readonly EmojiGroup beardedMan = Skin(char.ConvertFromUtf32(0x1F9D4), "Bearded Man");
        public static readonly Emoji manInSuitLevitating = new(char.ConvertFromUtf32(0x1F574) + emojiStyle.Value, "Man in Suit, Levitating");
        public static readonly EmojiGroup manWithChineseCap = Skin(char.ConvertFromUtf32(0x1F472), "Man With Chinese Cap");
        public static readonly EmojiGroup manInTuxedo = Skin(char.ConvertFromUtf32(0x1F935), "Man in Tuxedo");
        public static readonly EmojiGroup man = SkinAndHair(char.ConvertFromUtf32(0x1F468), "Man",
            blondes.Alts[1],
            beardedMan,
            manInSuitLevitating,
            manWithChineseCap,
            wearingTurban.Alts[0],
            manInTuxedo);

        public static readonly EmojiGroup pregnantWoman = Skin(char.ConvertFromUtf32(0x1F930), "Pregnant Woman");
        public static readonly EmojiGroup breastFeeding = Skin(char.ConvertFromUtf32(0x1F931), "Breast-Feeding");
        public static readonly EmojiGroup womanWithHeadscarf = Skin(char.ConvertFromUtf32(0x1F9D5), "Woman With Headscarf");
        public static readonly EmojiGroup brideWithVeil = Skin(char.ConvertFromUtf32(0x1F470), "Bride With Veil");
        public static readonly EmojiGroup woman = SkinAndHair(char.ConvertFromUtf32(0x1F469), "Woman",
            blondes.Alts[2],
            pregnantWoman,
            breastFeeding,
            womanWithHeadscarf,
            wearingTurban.Alts[1],
            brideWithVeil);

        public static readonly EmojiGroup adults = new(person.Value, "Adult", person, man, woman);

        public static readonly EmojiGroup olderPerson = Skin(char.ConvertFromUtf32(0x1F9D3), "Older Person");
        public static readonly EmojiGroup oldMan = Skin(char.ConvertFromUtf32(0x1F474), "Old Man");
        public static readonly EmojiGroup oldWoman = Skin(char.ConvertFromUtf32(0x1F475), "Old Woman");
        public static readonly EmojiGroup elderly = new(olderPerson, olderPerson, oldMan, oldWoman);

        public static readonly Emoji medical = new("\u2695" + emojiStyle.Value, "Medical");
        public static readonly EmojiGroup healthCareWorkers = Symbol(medical, "Health Care");

        public static readonly Emoji graduationCap = new(char.ConvertFromUtf32(0x1F393), "Graduation Cap");
        public static readonly EmojiGroup students = Symbol(graduationCap, "Student");

        public static readonly Emoji school = new(char.ConvertFromUtf32(0x1F3EB), "School");
        public static readonly EmojiGroup teachers = Symbol(school, "Teacher");

        public static readonly Emoji balanceScale = new("\u2696" + emojiStyle.Value, "Balance Scale");
        public static readonly EmojiGroup judges = Symbol(balanceScale, "Judge");

        public static readonly Emoji sheafOfRice = new(char.ConvertFromUtf32(0x1F33E), "Sheaf of Rice");
        public static readonly EmojiGroup farmers = Symbol(sheafOfRice, "Farmer");

        public static readonly Emoji cooking = new(char.ConvertFromUtf32(0x1F373), "Cooking");
        public static readonly EmojiGroup cooks = Symbol(cooking, "Cook");

        public static readonly Emoji wrench = new(char.ConvertFromUtf32(0x1F527), "Wrench");
        public static readonly EmojiGroup mechanics = Symbol(wrench, "Mechanic");

        public static readonly Emoji factory = new(char.ConvertFromUtf32(0x1F3ED), "Factory");
        public static readonly EmojiGroup factoryWorkers = Symbol(factory, "Factory Worker");

        public static readonly Emoji briefcase = new(char.ConvertFromUtf32(0x1F4BC), "Briefcase");
        public static readonly EmojiGroup officeWorkers = Symbol(briefcase, "Office Worker");

        public static readonly Emoji fireEngine = new(char.ConvertFromUtf32(0x1F692), "Fire Engine");
        public static readonly EmojiGroup fireFighters = Symbol(fireEngine, "Fire Fighter");

        public static readonly Emoji rocket = new(char.ConvertFromUtf32(0x1F680), "Rocket");
        public static readonly EmojiGroup astronauts = Symbol(rocket, "Astronaut");

        public static readonly Emoji airplane = new("\u2708" + emojiStyle.Value, "Airplane");
        public static readonly EmojiGroup pilots = Symbol(airplane, "Pilot");

        public static readonly Emoji artistPalette = new(char.ConvertFromUtf32(0x1F3A8), "Artist Palette");
        public static readonly EmojiGroup artists = Symbol(artistPalette, "Artist");

        public static readonly Emoji microphone = new(char.ConvertFromUtf32(0x1F3A4), "Microphone");
        public static readonly EmojiGroup singers = Symbol(microphone, "Singer");

        public static readonly Emoji laptop = new(char.ConvertFromUtf32(0x1F4BB), "Laptop");
        public static readonly EmojiGroup technologists = Symbol(laptop, "Technologist");

        public static readonly Emoji microscope = new(char.ConvertFromUtf32(0x1F52C), "Microscope");
        public static readonly EmojiGroup scientists = Symbol(microscope, "Scientist");

        public static readonly Emoji crown = new(char.ConvertFromUtf32(0x1F451), "Crown");
        public static readonly EmojiGroup prince = Skin(char.ConvertFromUtf32(0x1F934), "Prince");
        public static readonly EmojiGroup princess = Skin(char.ConvertFromUtf32(0x1F478), "Princess");
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

        public static readonly EmojiGroup cherub = Skin(char.ConvertFromUtf32(0x1F47C), "Cherub");
        public static readonly EmojiGroup santaClaus = Skin(char.ConvertFromUtf32(0x1F385), "Santa Claus");
        public static readonly EmojiGroup mrsClaus = Skin(char.ConvertFromUtf32(0x1F936), "Mrs. Claus");

        public static readonly Emoji genie = new(char.ConvertFromUtf32(0x1F9DE), "Genie");
        public static readonly EmojiGroup genies = Sex(genie);
        public static readonly Emoji zombie = new(char.ConvertFromUtf32(0x1F9DF), "Zombie");
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

        public static readonly Emoji whiteCane = new(char.ConvertFromUtf32(0x1F9AF), "Probing Cane");
        public static readonly EmojiGroup withProbingCane = Symbol(whiteCane, "Probing");

        public static readonly Emoji motorizedWheelchair = new(char.ConvertFromUtf32(0x1F9BC), "Motorized Wheelchair");
        public static readonly EmojiGroup inMotorizedWheelchair = Symbol(motorizedWheelchair, "In Motorized Wheelchair");

        public static readonly Emoji manualWheelchair = new(char.ConvertFromUtf32(0x1F9BD), "Manual Wheelchair");
        public static readonly EmojiGroup inManualWheelchair = Symbol(manualWheelchair, "In Manual Wheelchair");


        public static readonly EmojiGroup manDancing = Skin(char.ConvertFromUtf32(0x1F57A), "Man Dancing");
        public static readonly EmojiGroup womanDancing = Skin(char.ConvertFromUtf32(0x1F483), "Woman Dancing");
        public static readonly EmojiGroup dancers = new(manDancing.Value, "Dancing", manDancing, womanDancing);

        public static readonly EmojiGroup jugglers = SkinAndSex(char.ConvertFromUtf32(0x1F939), "Juggler");

        public static readonly EmojiGroup climbers = SkinAndSex(char.ConvertFromUtf32(0x1F9D7), "Climber");
        public static readonly Emoji fencer = new(char.ConvertFromUtf32(0x1F93A), "Fencer");
        public static readonly EmojiGroup jockeys = Skin(char.ConvertFromUtf32(0x1F3C7), "Jockey");
        public static readonly Emoji skier = new("\u26F7" + emojiStyle.Value, "Skier");
        public static readonly EmojiGroup snowboarders = Skin(char.ConvertFromUtf32(0x1F3C2), "Snowboarder");
        public static readonly EmojiGroup golfers = SkinAndSex(char.ConvertFromUtf32(0x1F3CC) + emojiStyle.Value, "Golfer");
        public static readonly EmojiGroup surfers = SkinAndSex(char.ConvertFromUtf32(0x1F3C4), "Surfing");
        public static readonly EmojiGroup rowers = SkinAndSex(char.ConvertFromUtf32(0x1F6A3), "Rowing Boat");
        public static readonly EmojiGroup swimmers = SkinAndSex(char.ConvertFromUtf32(0x1F3CA), "Swimming");
        public static readonly EmojiGroup basketballers = SkinAndSex("\u26F9" + emojiStyle.Value, "Basket Baller");
        public static readonly EmojiGroup weightLifters = SkinAndSex(char.ConvertFromUtf32(0x1F3CB) + emojiStyle.Value, "Weight Lifter");
        public static readonly EmojiGroup bikers = SkinAndSex(char.ConvertFromUtf32(0x1F6B4), "Biker");
        public static readonly EmojiGroup mountainBikers = SkinAndSex(char.ConvertFromUtf32(0x1F6B5), "Mountain Biker");
        public static readonly EmojiGroup cartwheelers = SkinAndSex(char.ConvertFromUtf32(0x1F938), "Cartwheeler");
        public static readonly Emoji wrestler = new(char.ConvertFromUtf32(0x1F93C), "Wrestler");
        public static readonly EmojiGroup wrestlers = Sex(wrestler);
        public static readonly EmojiGroup waterPoloers = SkinAndSex(char.ConvertFromUtf32(0x1F93D), "Water Polo Player");
        public static readonly EmojiGroup handBallers = SkinAndSex(char.ConvertFromUtf32(0x1F93E), "Hand Baller");

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

        public static readonly EmojiGroup inLotusPosition = SkinAndSex(char.ConvertFromUtf32(0x1F9D8), "In Lotus Position");
        public static readonly EmojiGroup inBath = Skin(char.ConvertFromUtf32(0x1F6C0), "In Bath");
        public static readonly EmojiGroup inBed = Skin(char.ConvertFromUtf32(0x1F6CC), "In Bed");
        public static readonly EmojiGroup inSauna = SkinAndSex(char.ConvertFromUtf32(0x1F9D6), "In Sauna");
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

        public static readonly Emoji ogre = new(char.ConvertFromUtf32(0x1F479), "Ogre");
        public static readonly Emoji goblin = new(char.ConvertFromUtf32(0x1F47A), "Goblin");
        public static readonly Emoji ghost = new(char.ConvertFromUtf32(0x1F47B), "Ghost");
        public static readonly Emoji alien = new(char.ConvertFromUtf32(0x1F47D), "Alien");
        public static readonly Emoji alienMonster = new(char.ConvertFromUtf32(0x1F47E), "Alien Monster");
        public static readonly Emoji angryFaceWithHorns = new(char.ConvertFromUtf32(0x1F47F), "Angry Face with Horns");
        public static readonly Emoji skull = new(char.ConvertFromUtf32(0x1F480), "Skull");
        public static readonly Emoji pileOfPoo = new(char.ConvertFromUtf32(0x1F4A9), "Pile of Poo");
        public static readonly Emoji grinningFace = new(char.ConvertFromUtf32(0x1F600), "Grinning Face");
        public static readonly Emoji beamingFaceWithSmilingEyes = new(char.ConvertFromUtf32(0x1F601), "Beaming Face with Smiling Eyes");
        public static readonly Emoji faceWithTearsOfJoy = new(char.ConvertFromUtf32(0x1F602), "Face with Tears of Joy");
        public static readonly Emoji grinningFaceWithBigEyes = new(char.ConvertFromUtf32(0x1F603), "Grinning Face with Big Eyes");
        public static readonly Emoji grinningFaceWithSmilingEyes = new(char.ConvertFromUtf32(0x1F604), "Grinning Face with Smiling Eyes");
        public static readonly Emoji grinningFaceWithSweat = new(char.ConvertFromUtf32(0x1F605), "Grinning Face with Sweat");
        public static readonly Emoji grinningSquitingFace = new(char.ConvertFromUtf32(0x1F606), "Grinning Squinting Face");
        public static readonly Emoji smillingFaceWithHalo = new(char.ConvertFromUtf32(0x1F607), "Smiling Face with Halo");
        public static readonly Emoji smilingFaceWithHorns = new(char.ConvertFromUtf32(0x1F608), "Smiling Face with Horns");
        public static readonly Emoji winkingFace = new(char.ConvertFromUtf32(0x1F609), "Winking Face");
        public static readonly Emoji smilingFaceWithSmilingEyes = new(char.ConvertFromUtf32(0x1F60A), "Smiling Face with Smiling Eyes");
        public static readonly Emoji faceSavoringFood = new(char.ConvertFromUtf32(0x1F60B), "Face Savoring Food");
        public static readonly Emoji relievedFace = new(char.ConvertFromUtf32(0x1F60C), "Relieved Face");
        public static readonly Emoji smilingFaceWithHeartEyes = new(char.ConvertFromUtf32(0x1F60D), "Smiling Face with Heart-Eyes");
        public static readonly Emoji smilingFaceWithSunglasses = new(char.ConvertFromUtf32(0x1F60E), "Smiling Face with Sunglasses");
        public static readonly Emoji smirkingFace = new(char.ConvertFromUtf32(0x1F60F), "Smirking Face");
        public static readonly Emoji neutralFace = new(char.ConvertFromUtf32(0x1F610), "Neutral Face");
        public static readonly Emoji expressionlessFace = new(char.ConvertFromUtf32(0x1F611), "Expressionless Face");
        public static readonly Emoji unamusedFace = new(char.ConvertFromUtf32(0x1F612), "Unamused Face");
        public static readonly Emoji downcastFaceWithSweat = new(char.ConvertFromUtf32(0x1F613), "Downcast Face with Sweat");
        public static readonly Emoji pensiveFace = new(char.ConvertFromUtf32(0x1F614), "Pensive Face");
        public static readonly Emoji confusedFace = new(char.ConvertFromUtf32(0x1F615), "Confused Face");
        public static readonly Emoji confoundedFace = new(char.ConvertFromUtf32(0x1F616), "Confounded Face");
        public static readonly Emoji kissingFace = new(char.ConvertFromUtf32(0x1F617), "Kissing Face");
        public static readonly Emoji faceBlowingAKiss = new(char.ConvertFromUtf32(0x1F618), "Face Blowing a Kiss");
        public static readonly Emoji kissingFaceWithSmilingEyes = new(char.ConvertFromUtf32(0x1F619), "Kissing Face with Smiling Eyes");
        public static readonly Emoji kissingFaceWithClosedEyes = new(char.ConvertFromUtf32(0x1F61A), "Kissing Face with Closed Eyes");
        public static readonly Emoji faceWithTongue = new(char.ConvertFromUtf32(0x1F61B), "Face with Tongue");
        public static readonly Emoji winkingFaceWithTongue = new(char.ConvertFromUtf32(0x1F61C), "Winking Face with Tongue");
        public static readonly Emoji squintingFaceWithTongue = new(char.ConvertFromUtf32(0x1F61D), "Squinting Face with Tongue");
        public static readonly Emoji disappointedFace = new(char.ConvertFromUtf32(0x1F61E), "Disappointed Face");
        public static readonly Emoji worriedFace = new(char.ConvertFromUtf32(0x1F61F), "Worried Face");
        public static readonly Emoji angryFace = new(char.ConvertFromUtf32(0x1F620), "Angry Face");
        public static readonly Emoji poutingFace = new(char.ConvertFromUtf32(0x1F621), "Pouting Face");
        public static readonly Emoji cryingFace = new(char.ConvertFromUtf32(0x1F622), "Crying Face");
        public static readonly Emoji perseveringFace = new(char.ConvertFromUtf32(0x1F623), "Persevering Face");
        public static readonly Emoji faceWithSteamFromNose = new(char.ConvertFromUtf32(0x1F624), "Face with Steam From Nose");
        public static readonly Emoji sadButRelievedFace = new(char.ConvertFromUtf32(0x1F625), "Sad but Relieved Face");
        public static readonly Emoji frowningFaceWithOpenMouth = new(char.ConvertFromUtf32(0x1F626), "Frowning Face with Open Mouth");
        public static readonly Emoji anguishedFace = new(char.ConvertFromUtf32(0x1F627), "Anguished Face");
        public static readonly Emoji fearfulFace = new(char.ConvertFromUtf32(0x1F628), "Fearful Face");
        public static readonly Emoji wearyFace = new(char.ConvertFromUtf32(0x1F629), "Weary Face");
        public static readonly Emoji sleepyFace = new(char.ConvertFromUtf32(0x1F62A), "Sleepy Face");
        public static readonly Emoji tiredFace = new(char.ConvertFromUtf32(0x1F62B), "Tired Face");
        public static readonly Emoji grimacingFace = new(char.ConvertFromUtf32(0x1F62C), "Grimacing Face");
        public static readonly Emoji loudlyCryingFace = new(char.ConvertFromUtf32(0x1F62D), "Loudly Crying Face");
        public static readonly Emoji faceWithOpenMouth = new(char.ConvertFromUtf32(0x1F62E), "Face with Open Mouth");
        public static readonly Emoji hushedFace = new(char.ConvertFromUtf32(0x1F62F), "Hushed Face");
        public static readonly Emoji anxiousFaceWithSweat = new(char.ConvertFromUtf32(0x1F630), "Anxious Face with Sweat");
        public static readonly Emoji faceScreamingInFear = new(char.ConvertFromUtf32(0x1F631), "Face Screaming in Fear");
        public static readonly Emoji astonishedFace = new(char.ConvertFromUtf32(0x1F632), "Astonished Face");
        public static readonly Emoji flushedFace = new(char.ConvertFromUtf32(0x1F633), "Flushed Face");
        public static readonly Emoji sleepingFace = new(char.ConvertFromUtf32(0x1F634), "Sleeping Face");
        public static readonly Emoji dizzyFace = new(char.ConvertFromUtf32(0x1F635), "Dizzy Face");
        public static readonly Emoji faceWithoutMouth = new(char.ConvertFromUtf32(0x1F636), "Face Without Mouth");
        public static readonly Emoji faceWithMedicalMask = new(char.ConvertFromUtf32(0x1F637), "Face with Medical Mask");
        public static readonly Emoji grinningCatWithSmilingEyes = new(char.ConvertFromUtf32(0x1F638), "Grinning Cat with Smiling Eyes");
        public static readonly Emoji catWithTearsOfJoy = new(char.ConvertFromUtf32(0x1F639), "Cat with Tears of Joy");
        public static readonly Emoji grinningCat = new(char.ConvertFromUtf32(0x1F63A), "Grinning Cat");
        public static readonly Emoji smilingCatWithHeartEyes = new(char.ConvertFromUtf32(0x1F63B), "Smiling Cat with Heart-Eyes");
        public static readonly Emoji catWithWrySmile = new(char.ConvertFromUtf32(0x1F63C), "Cat with Wry Smile");
        public static readonly Emoji kissingCat = new(char.ConvertFromUtf32(0x1F63D), "Kissing Cat");
        public static readonly Emoji poutingCat = new(char.ConvertFromUtf32(0x1F63E), "Pouting Cat");
        public static readonly Emoji cryingCat = new(char.ConvertFromUtf32(0x1F63F), "Crying Cat");
        public static readonly Emoji wearyCat = new(char.ConvertFromUtf32(0x1F640), "Weary Cat");
        public static readonly Emoji slightlyFrowningFace = new(char.ConvertFromUtf32(0x1F641), "Slightly Frowning Face");
        public static readonly Emoji slightlySmilingFace = new(char.ConvertFromUtf32(0x1F642), "Slightly Smiling Face");
        public static readonly Emoji updisdeDownFace = new(char.ConvertFromUtf32(0x1F643), "Upside-Down Face");
        public static readonly Emoji faceWithRollingEyes = new(char.ConvertFromUtf32(0x1F644), "Face with Rolling Eyes");
        public static readonly Emoji seeNoEvilMonkey = new(char.ConvertFromUtf32(0x1F648), "See-No-Evil Monkey");
        public static readonly Emoji hearNoEvilMonkey = new(char.ConvertFromUtf32(0x1F649), "Hear-No-Evil Monkey");
        public static readonly Emoji speakNoEvilMonkey = new(char.ConvertFromUtf32(0x1F64A), "Speak-No-Evil Monkey");
        public static readonly Emoji zipperMouthFace = new(char.ConvertFromUtf32(0x1F910), "Zipper-Mouth Face");
        public static readonly Emoji moneyMouthFace = new(char.ConvertFromUtf32(0x1F911), "Money-Mouth Face");
        public static readonly Emoji faceWithThermometer = new(char.ConvertFromUtf32(0x1F912), "Face with Thermometer");
        public static readonly Emoji nerdFace = new(char.ConvertFromUtf32(0x1F913), "Nerd Face");
        public static readonly Emoji thinkingFace = new(char.ConvertFromUtf32(0x1F914), "Thinking Face");
        public static readonly Emoji faceWithHeadBandage = new(char.ConvertFromUtf32(0x1F915), "Face with Head-Bandage");
        public static readonly Emoji robot = new(char.ConvertFromUtf32(0x1F916), "Robot");
        public static readonly Emoji huggingFace = new(char.ConvertFromUtf32(0x1F917), "Hugging Face");
        public static readonly Emoji cowboyHatFace = new(char.ConvertFromUtf32(0x1F920), "Cowboy Hat Face");
        public static readonly Emoji clownFace = new(char.ConvertFromUtf32(0x1F921), "Clown Face");
        public static readonly Emoji nauseatedFace = new(char.ConvertFromUtf32(0x1F922), "Nauseated Face");
        public static readonly Emoji rollingOnTheFloorLaughing = new(char.ConvertFromUtf32(0x1F923), "Rolling on the Floor Laughing");
        public static readonly Emoji droolingFace = new(char.ConvertFromUtf32(0x1F924), "Drooling Face");
        public static readonly Emoji lyingFace = new(char.ConvertFromUtf32(0x1F925), "Lying Face");
        public static readonly Emoji sneezingFace = new(char.ConvertFromUtf32(0x1F927), "Sneezing Face");
        public static readonly Emoji faceWithRaisedEyebrow = new(char.ConvertFromUtf32(0x1F928), "Face with Raised Eyebrow");
        public static readonly Emoji starStruck = new(char.ConvertFromUtf32(0x1F929), "Star-Struck");
        public static readonly Emoji zanyFace = new(char.ConvertFromUtf32(0x1F92A), "Zany Face");
        public static readonly Emoji shushingFace = new(char.ConvertFromUtf32(0x1F92B), "Shushing Face");
        public static readonly Emoji faceWithSymbolsOnMouth = new(char.ConvertFromUtf32(0x1F92C), "Face with Symbols on Mouth");
        public static readonly Emoji faceWithHandOverMouth = new(char.ConvertFromUtf32(0x1F92D), "Face with Hand Over Mouth");
        public static readonly Emoji faceVomitting = new(char.ConvertFromUtf32(0x1F92E), "Face Vomiting");
        public static readonly Emoji explodingHead = new(char.ConvertFromUtf32(0x1F92F), "Exploding Head");
        public static readonly Emoji smilingFaceWithHearts = new(char.ConvertFromUtf32(0x1F970), "Smiling Face with Hearts");
        public static readonly Emoji yawningFace = new(char.ConvertFromUtf32(0x1F971), "Yawning Face");
        public static readonly Emoji smilingFaceWithTear = new(char.ConvertFromUtf32(0x1F972), "Smiling Face with Tear");
        public static readonly Emoji partyingFace = new(char.ConvertFromUtf32(0x1F973), "Partying Face");
        public static readonly Emoji woozyFace = new(char.ConvertFromUtf32(0x1F974), "Woozy Face");
        public static readonly Emoji hotFace = new(char.ConvertFromUtf32(0x1F975), "Hot Face");
        public static readonly Emoji coldFace = new(char.ConvertFromUtf32(0x1F976), "Cold Face");
        public static readonly Emoji disguisedFace = new(char.ConvertFromUtf32(0x1F978), "Disguised Face");
        public static readonly Emoji pleadingFace = new(char.ConvertFromUtf32(0x1F97A), "Pleading Face");
        public static readonly Emoji faceWithMonocle = new(char.ConvertFromUtf32(0x1F9D0), "Face with Monocle");
        public static readonly Emoji skullAndCrossbones = new("\u2620" + emojiStyle.Value, "Skull and Crossbones");
        public static readonly Emoji frowningFace = new("\u2639" + emojiStyle.Value, "Frowning Face");
        public static readonly Emoji smilingFace = new("\u263A" + emojiStyle.Value, "Smiling Face");
        public static readonly Emoji speakingHead = new(char.ConvertFromUtf32(0x1F5E3) + emojiStyle.Value, "Speaking Head");
        public static readonly Emoji bust = new(char.ConvertFromUtf32(0x1F464), "Bust in Silhouette");
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

        public static readonly Emoji kissMark = new(char.ConvertFromUtf32(0x1F48B), "Kiss Mark");
        public static readonly Emoji loveLetter = new(char.ConvertFromUtf32(0x1F48C), "Love Letter");
        public static readonly Emoji beatingHeart = new(char.ConvertFromUtf32(0x1F493), "Beating Heart");
        public static readonly Emoji brokenHeart = new(char.ConvertFromUtf32(0x1F494), "Broken Heart");
        public static readonly Emoji twoHearts = new(char.ConvertFromUtf32(0x1F495), "Two Hearts");
        public static readonly Emoji sparklingHeart = new(char.ConvertFromUtf32(0x1F496), "Sparkling Heart");
        public static readonly Emoji growingHeart = new(char.ConvertFromUtf32(0x1F497), "Growing Heart");
        public static readonly Emoji heartWithArrow = new(char.ConvertFromUtf32(0x1F498), "Heart with Arrow");
        public static readonly Emoji blueHeart = new(char.ConvertFromUtf32(0x1F499), "Blue Heart");
        public static readonly Emoji greenHeart = new(char.ConvertFromUtf32(0x1F49A), "Green Heart");
        public static readonly Emoji yellowHeart = new(char.ConvertFromUtf32(0x1F49B), "Yellow Heart");
        public static readonly Emoji purpleHeart = new(char.ConvertFromUtf32(0x1F49C), "Purple Heart");
        public static readonly Emoji heartWithRibbon = new(char.ConvertFromUtf32(0x1F49D), "Heart with Ribbon");
        public static readonly Emoji revolvingHearts = new(char.ConvertFromUtf32(0x1F49E), "Revolving Hearts");
        public static readonly Emoji heartDecoration = new(char.ConvertFromUtf32(0x1F49F), "Heart Decoration");
        public static readonly Emoji blackHeart = new(char.ConvertFromUtf32(0x1F5A4), "Black Heart");
        public static readonly Emoji whiteHeart = new(char.ConvertFromUtf32(0x1F90D), "White Heart");
        public static readonly Emoji brownHeart = new(char.ConvertFromUtf32(0x1F90E), "Brown Heart");
        public static readonly Emoji orangeHeart = new(char.ConvertFromUtf32(0x1F9E1), "Orange Heart");
        public static readonly Emoji heartExclamation = new("\u2763" + emojiStyle.Value, "Heart Exclamation");
        public static readonly Emoji redHeart = new("\u2764" + emojiStyle.Value, "Red Heart");
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

        public static readonly Emoji angerSymbol = new(char.ConvertFromUtf32(0x1F4A2), "Anger Symbol");
        public static readonly Emoji bomb = new(char.ConvertFromUtf32(0x1F4A3), "Bomb");
        public static readonly Emoji zzz = new(char.ConvertFromUtf32(0x1F4A4), "Zzz");
        public static readonly Emoji collision = new(char.ConvertFromUtf32(0x1F4A5), "Collision");
        public static readonly Emoji sweatDroplets = new(char.ConvertFromUtf32(0x1F4A6), "Sweat Droplets");
        public static readonly Emoji dashingAway = new(char.ConvertFromUtf32(0x1F4A8), "Dashing Away");
        public static readonly Emoji dizzy = new(char.ConvertFromUtf32(0x1F4AB), "Dizzy");
        public static readonly Emoji speechBalloon = new(char.ConvertFromUtf32(0x1F4AC), "Speech Balloon");
        public static readonly Emoji thoughtBalloon = new(char.ConvertFromUtf32(0x1F4AD), "Thought Balloon");
        public static readonly Emoji hundredPoints = new(char.ConvertFromUtf32(0x1F4AF), "Hundred Points");
        public static readonly Emoji hole = new(char.ConvertFromUtf32(0x1F573) + emojiStyle.Value, "Hole");
        public static readonly Emoji leftSpeechBubble = new(char.ConvertFromUtf32(0x1F5E8) + emojiStyle.Value, "Left Speech Bubble");
        public static readonly Emoji rightSpeechBubble = new(char.ConvertFromUtf32(0x1F5E9) + emojiStyle.Value, "Right Speech Bubble");
        public static readonly Emoji conversationBubbles2 = new(char.ConvertFromUtf32(0x1F5EA) + emojiStyle.Value, "Conversation Bubbles 2");
        public static readonly Emoji conversationBubbles3 = new(char.ConvertFromUtf32(0x1F5EB) + emojiStyle.Value, "Conversation Bubbles 3");
        public static readonly Emoji leftThoughtBubble = new(char.ConvertFromUtf32(0x1F5EC) + emojiStyle.Value, "Left Thought Bubble");
        public static readonly Emoji rightThoughtBubble = new(char.ConvertFromUtf32(0x1F5ED) + emojiStyle.Value, "Right Thought Bubble");
        public static readonly Emoji leftAngerBubble = new(char.ConvertFromUtf32(0x1F5EE) + emojiStyle.Value, "Left Anger Bubble");
        public static readonly Emoji rightAngerBubble = new(char.ConvertFromUtf32(0x1F5EF) + emojiStyle.Value, "Right Anger Bubble");
        public static readonly Emoji angerBubble = new(char.ConvertFromUtf32(0x1F5F0) + emojiStyle.Value, "Anger Bubble");
        public static readonly Emoji angerBubbleLightningBolt = new(char.ConvertFromUtf32(0x1F5F1) + emojiStyle.Value, "Anger Bubble Lightning");
        public static readonly Emoji lightningBolt = new(char.ConvertFromUtf32(0x1F5F2) + emojiStyle.Value, "Lightning Bolt");

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

        public static readonly Emoji backhandIndexPointingUp = new(char.ConvertFromUtf32(0x1F446), "Backhand Index Pointing Up");
        public static readonly Emoji backhandIndexPointingDown = new(char.ConvertFromUtf32(0x1F447), "Backhand Index Pointing Down");
        public static readonly Emoji backhandIndexPointingLeft = new(char.ConvertFromUtf32(0x1F448), "Backhand Index Pointing Left");
        public static readonly Emoji backhandIndexPointingRight = new(char.ConvertFromUtf32(0x1F449), "Backhand Index Pointing Right");
        public static readonly Emoji oncomingFist = new(char.ConvertFromUtf32(0x1F44A), "Oncoming Fist");
        public static readonly Emoji wavingHand = new(char.ConvertFromUtf32(0x1F44B), "Waving Hand");
        public static readonly Emoji okHand = new(char.ConvertFromUtf32(0x1F58F), "OK Hand");
        public static readonly Emoji thumbsUp = new(char.ConvertFromUtf32(0x1F44D), "Thumbs Up");
        public static readonly Emoji thumbsDown = new(char.ConvertFromUtf32(0x1F44E), "Thumbs Down");
        public static readonly Emoji clappingHands = new(char.ConvertFromUtf32(0x1F44F), "Clapping Hands");
        public static readonly Emoji openHands = new(char.ConvertFromUtf32(0x1F450), "Open Hands");
        public static readonly Emoji nailPolish = new(char.ConvertFromUtf32(0x1F485), "Nail Polish");
        public static readonly Emoji handsWithFingersSplayed = new(char.ConvertFromUtf32(0x1F590) + emojiStyle.Value, "Hand with Fingers Splayed");
        public static readonly Emoji handsWithFingersSplayed2 = new(char.ConvertFromUtf32(0x1F591) + emojiStyle.Value, "Hand with Fingers Splayed 2");
        public static readonly Emoji thumbsUp2 = new(char.ConvertFromUtf32(0x1F592), "Thumbs Up 2");
        public static readonly Emoji thumbsDown2 = new(char.ConvertFromUtf32(0x1F593), "Thumbs Down 2");
        public static readonly Emoji peaceFingers = new(char.ConvertFromUtf32(0x1F594), "Peace Fingers");
        public static readonly Emoji middleFinger = new(char.ConvertFromUtf32(0x1F595), "Middle Finger");
        public static readonly Emoji vulcanSalute = new(char.ConvertFromUtf32(0x1F596), "Vulcan Salute");
        public static readonly Emoji handPointingDown = new(char.ConvertFromUtf32(0x1F597), "Hand Pointing Down");
        public static readonly Emoji handPointingLeft = new(char.ConvertFromUtf32(0x1F598), "Hand Pointing Left");
        public static readonly Emoji handPointingRight = new(char.ConvertFromUtf32(0x1F599), "Hand Pointing Right");
        public static readonly Emoji handPointingLeft2 = new(char.ConvertFromUtf32(0x1F59A), "Hand Pointing Left 2");
        public static readonly Emoji handPointingRight2 = new(char.ConvertFromUtf32(0x1F59B), "Hand Pointing Right 2");
        public static readonly Emoji indexPointingLeft = new(char.ConvertFromUtf32(0x1F59C), "Index Pointing Left");
        public static readonly Emoji indexPointingRight = new(char.ConvertFromUtf32(0x1F59D), "Index Pointing Right");
        public static readonly Emoji indexPointingUp = new(char.ConvertFromUtf32(0x1F59E), "Index Pointing Up");
        public static readonly Emoji indexPointingDown = new(char.ConvertFromUtf32(0x1F59F), "Index Pointing Down");
        public static readonly Emoji indexPointingUp2 = new(char.ConvertFromUtf32(0x1F5A0), "Index Pointing Up 2");
        public static readonly Emoji indexPointingDown2 = new(char.ConvertFromUtf32(0x1F5A1), "Index Pointing Down 2");
        public static readonly Emoji indexPointingUp3 = new(char.ConvertFromUtf32(0x1F5A2), "Index Pointing Up 3");
        public static readonly Emoji indexPointingDown3 = new(char.ConvertFromUtf32(0x1F5A3), "Index Pointing Down 3");
        public static readonly Emoji raisingHands = new(char.ConvertFromUtf32(0x1F64C), "Raising Hands");
        public static readonly Emoji foldedHands = new(char.ConvertFromUtf32(0x1F64F), "Folded Hands");
        public static readonly Emoji pinchedFingers = new(char.ConvertFromUtf32(0x1F90C), "Pinched Fingers");
        public static readonly Emoji pinchingHand = new(char.ConvertFromUtf32(0x1F90F), "Pinching Hand");
        public static readonly Emoji signOfTheHorns = new(char.ConvertFromUtf32(0x1F918), "Sign of the Horns");
        public static readonly Emoji callMeHand = new(char.ConvertFromUtf32(0x1F919), "Call Me Hand");
        public static readonly Emoji rasiedBackOfHand = new(char.ConvertFromUtf32(0x1F91A), "Raised Back of Hand");
        public static readonly Emoji leftFacingFist = new(char.ConvertFromUtf32(0x1F91B), "Left-Facing Fist");
        public static readonly Emoji rightFacingFist = new(char.ConvertFromUtf32(0x1F91C), "Right-Facing Fist");
        public static readonly Emoji handshake = new(char.ConvertFromUtf32(0x1F91D), "Handshake");
        public static readonly Emoji crossedFingers = new(char.ConvertFromUtf32(0x1F91E), "Crossed Fingers");
        public static readonly Emoji loveYouGesture = new(char.ConvertFromUtf32(0x1F91F), "Love-You Gesture");
        public static readonly Emoji palmsUpTogether = new(char.ConvertFromUtf32(0x1F932), "Palms Up Together");
        public static readonly Emoji indexPointingUp4 = new("\u261D" + emojiStyle.Value, "Index Pointing Up 4");
        public static readonly Emoji raisedFist = new("\u270A", "Raised Fist");
        public static readonly Emoji raisedHand = new("\u270B", "Raised Hand");
        public static readonly Emoji victoryHand = new("\u270C" + emojiStyle.Value, "Victory Hand");
        public static readonly Emoji writingHand = new("\u270D" + emojiStyle.Value, "Writing Hand");
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
            new(char.ConvertFromUtf32(0x1F440), "Eyes"),
            new(char.ConvertFromUtf32(0x1F441) + emojiStyle.Value, "Eye"),
            new(char.ConvertFromUtf32(0x1F441) + emojiStyle.Value + zeroWidthJoiner.Value + char.ConvertFromUtf32(0x1F5E8) + emojiStyle.Value, "Eye in Speech Bubble"),
            new(char.ConvertFromUtf32(0x1F442), "Ear"),
            new(char.ConvertFromUtf32(0x1F443), "Nose"),
            new(char.ConvertFromUtf32(0x1F444), "Mouth"),
            new(char.ConvertFromUtf32(0x1F445), "Tongue"),
            new(char.ConvertFromUtf32(0x1F4AA), "Flexed Biceps"),
            new(char.ConvertFromUtf32(0x1F933), "Selfie"),
            new(char.ConvertFromUtf32(0x1F9B4), "Bone"),
            new(char.ConvertFromUtf32(0x1F9B5), "Leg"),
            new(char.ConvertFromUtf32(0x1F9B6), "Foot"),
            new(char.ConvertFromUtf32(0x1F9B7), "Tooth"),
            new(char.ConvertFromUtf32(0x1F9BB), "Ear with Hearing Aid"),
            new(char.ConvertFromUtf32(0x1F9BE), "Mechanical Arm"),
            new(char.ConvertFromUtf32(0x1F9BF), "Mechanical Leg"),
            new(char.ConvertFromUtf32(0x1FAC0), "Anatomical Heart"),
            new(char.ConvertFromUtf32(0x1FAC1), "Lungs"),
            new(char.ConvertFromUtf32(0x1F9E0), "Brain"));

        public static readonly Emoji whiteFlower = new(char.ConvertFromUtf32(0x1F4AE), "White Flower");
        public static readonly EmojiGroup plants = new(
            "Plants", "Flowers, trees, and things",
            new(char.ConvertFromUtf32(0x1F331), "Seedling"),
            new(char.ConvertFromUtf32(0x1F332), "Evergreen Tree"),
            new(char.ConvertFromUtf32(0x1F333), "Deciduous Tree"),
            new(char.ConvertFromUtf32(0x1F334), "Palm Tree"),
            new(char.ConvertFromUtf32(0x1F335), "Cactus"),
            new(char.ConvertFromUtf32(0x1F337), "Tulip"),
            new(char.ConvertFromUtf32(0x1F338), "Cherry Blossom"),
            new(char.ConvertFromUtf32(0x1F339), "Rose"),
            new(char.ConvertFromUtf32(0x1F33A), "Hibiscus"),
            new(char.ConvertFromUtf32(0x1F33B), "Sunflower"),
            new(char.ConvertFromUtf32(0x1F33C), "Blossom"),
            sheafOfRice,
            new(char.ConvertFromUtf32(0x1F33F), "Herb"),
            new(char.ConvertFromUtf32(0x1F340), "Four Leaf Clover"),
            new(char.ConvertFromUtf32(0x1F341), "Maple Leaf"),
            new(char.ConvertFromUtf32(0x1F342), "Fallen Leaf"),
            new(char.ConvertFromUtf32(0x1F343), "Leaf Fluttering in Wind"),
            new(char.ConvertFromUtf32(0x1F3F5) + emojiStyle.Value, "Rosette"),
            new(char.ConvertFromUtf32(0x1F490), "Bouquet"),
            whiteFlower,
            new(char.ConvertFromUtf32(0x1F940), "Wilted Flower"),
            new(char.ConvertFromUtf32(0x1FAB4), "Potted Plant"),
            new("\u2618" + emojiStyle.Value, "Shamrock"));

        public static readonly Emoji banana = new(char.ConvertFromUtf32(0x1F34C), "Banana");
        public static readonly EmojiGroup food = new(
            "Food", "Food, drink, and utensils",
            new(char.ConvertFromUtf32(0x1F32D), "Hot Dog"),
            new(char.ConvertFromUtf32(0x1F32E), "Taco"),
            new(char.ConvertFromUtf32(0x1F32F), "Burrito"),
            new(char.ConvertFromUtf32(0x1F330), "Chestnut"),
            new(char.ConvertFromUtf32(0x1F336) + emojiStyle.Value, "Hot Pepper"),
            new(char.ConvertFromUtf32(0x1F33D), "Ear of Corn"),
            new(char.ConvertFromUtf32(0x1F344), "Mushroom"),
            new(char.ConvertFromUtf32(0x1F345), "Tomato"),
            new(char.ConvertFromUtf32(0x1F346), "Eggplant"),
            new(char.ConvertFromUtf32(0x1F347), "Grapes"),
            new(char.ConvertFromUtf32(0x1F348), "Melon"),
            new(char.ConvertFromUtf32(0x1F349), "Watermelon"),
            new(char.ConvertFromUtf32(0x1F34A), "Tangerine"),
            new(char.ConvertFromUtf32(0x1F34B), "Lemon"),
            banana,
            new(char.ConvertFromUtf32(0x1F34D), "Pineapple"),
            new(char.ConvertFromUtf32(0x1F34E), "Red Apple"),
            new(char.ConvertFromUtf32(0x1F34F), "Green Apple"),
            new(char.ConvertFromUtf32(0x1F350), "Pear"),
            new(char.ConvertFromUtf32(0x1F351), "Peach"),
            new(char.ConvertFromUtf32(0x1F352), "Cherries"),
            new(char.ConvertFromUtf32(0x1F353), "Strawberry"),
            new(char.ConvertFromUtf32(0x1F354), "Hamburger"),
            new(char.ConvertFromUtf32(0x1F355), "Pizza"),
            new(char.ConvertFromUtf32(0x1F356), "Meat on Bone"),
            new(char.ConvertFromUtf32(0x1F357), "Poultry Leg"),
            new(char.ConvertFromUtf32(0x1F358), "Rice Cracker"),
            new(char.ConvertFromUtf32(0x1F359), "Rice Ball"),
            new(char.ConvertFromUtf32(0x1F35A), "Cooked Rice"),
            new(char.ConvertFromUtf32(0x1F35B), "Curry Rice"),
            new(char.ConvertFromUtf32(0x1F35C), "Steaming Bowl"),
            new(char.ConvertFromUtf32(0x1F35D), "Spaghetti"),
            new(char.ConvertFromUtf32(0x1F35E), "Bread"),
            new(char.ConvertFromUtf32(0x1F35F), "French Fries"),
            new(char.ConvertFromUtf32(0x1F360), "Roasted Sweet Potato"),
            new(char.ConvertFromUtf32(0x1F361), "Dango"),
            new(char.ConvertFromUtf32(0x1F362), "Oden"),
            new(char.ConvertFromUtf32(0x1F363), "Sushi"),
            new(char.ConvertFromUtf32(0x1F364), "Fried Shrimp"),
            new(char.ConvertFromUtf32(0x1F365), "Fish Cake with Swirl"),
            new(char.ConvertFromUtf32(0x1F371), "Bento Box"),
            new(char.ConvertFromUtf32(0x1F372), "Pot of Food"),
            cooking,
            new(char.ConvertFromUtf32(0x1F37F), "Popcorn"),
            new(char.ConvertFromUtf32(0x1F950), "Croissant"),
            new(char.ConvertFromUtf32(0x1F951), "Avocado"),
            new(char.ConvertFromUtf32(0x1F952), "Cucumber"),
            new(char.ConvertFromUtf32(0x1F953), "Bacon"),
            new(char.ConvertFromUtf32(0x1F954), "Potato"),
            new(char.ConvertFromUtf32(0x1F955), "Carrot"),
            new(char.ConvertFromUtf32(0x1F956), "Baguette Bread"),
            new(char.ConvertFromUtf32(0x1F957), "Green Salad"),
            new(char.ConvertFromUtf32(0x1F958), "Shallow Pan of Food"),
            new(char.ConvertFromUtf32(0x1F959), "Stuffed Flatbread"),
            new(char.ConvertFromUtf32(0x1F95A), "Egg"),
            new(char.ConvertFromUtf32(0x1F95C), "Peanuts"),
            new(char.ConvertFromUtf32(0x1F95D), "Kiwi Fruit"),
            new(char.ConvertFromUtf32(0x1F95E), "Pancakes"),
            new(char.ConvertFromUtf32(0x1F95F), "Dumpling"),
            new(char.ConvertFromUtf32(0x1F960), "Fortune Cookie"),
            new(char.ConvertFromUtf32(0x1F961), "Takeout Box"),
            new(char.ConvertFromUtf32(0x1F963), "Bowl with Spoon"),
            new(char.ConvertFromUtf32(0x1F965), "Coconut"),
            new(char.ConvertFromUtf32(0x1F966), "Broccoli"),
            new(char.ConvertFromUtf32(0x1F968), "Pretzel"),
            new(char.ConvertFromUtf32(0x1F969), "Cut of Meat"),
            new(char.ConvertFromUtf32(0x1F96A), "Sandwich"),
            new(char.ConvertFromUtf32(0x1F96B), "Canned Food"),
            new(char.ConvertFromUtf32(0x1F96C), "Leafy Green"),
            new(char.ConvertFromUtf32(0x1F96D), "Mango"),
            new(char.ConvertFromUtf32(0x1F96E), "Moon Cake"),
            new(char.ConvertFromUtf32(0x1F96F), "Bagel"),
            new(char.ConvertFromUtf32(0x1F980), "Crab"),
            new(char.ConvertFromUtf32(0x1F990), "Shrimp"),
            new(char.ConvertFromUtf32(0x1F991), "Squid"),
            new(char.ConvertFromUtf32(0x1F99E), "Lobster"),
            new(char.ConvertFromUtf32(0x1F9AA), "Oyster"),
            new(char.ConvertFromUtf32(0x1F9C0), "Cheese Wedge"),
            new(char.ConvertFromUtf32(0x1F9C2), "Salt"),
            new(char.ConvertFromUtf32(0x1F9C4), "Garlic"),
            new(char.ConvertFromUtf32(0x1F9C5), "Onion"),
            new(char.ConvertFromUtf32(0x1F9C6), "Falafel"),
            new(char.ConvertFromUtf32(0x1F9C7), "Waffle"),
            new(char.ConvertFromUtf32(0x1F9C8), "Butter"),
            new(char.ConvertFromUtf32(0x1FAD0), "Blueberries"),
            new(char.ConvertFromUtf32(0x1FAD1), "Bell Pepper"),
            new(char.ConvertFromUtf32(0x1FAD2), "Olive"),
            new(char.ConvertFromUtf32(0x1FAD3), "Flatbread"),
            new(char.ConvertFromUtf32(0x1FAD4), "Tamale"),
            new(char.ConvertFromUtf32(0x1FAD5), "Fondue"),
            new(char.ConvertFromUtf32(0x1F366), "Soft Ice Cream"),
            new(char.ConvertFromUtf32(0x1F367), "Shaved Ice"),
            new(char.ConvertFromUtf32(0x1F368), "Ice Cream"),
            new(char.ConvertFromUtf32(0x1F369), "Doughnut"),
            new(char.ConvertFromUtf32(0x1F36A), "Cookie"),
            new(char.ConvertFromUtf32(0x1F36B), "Chocolate Bar"),
            new(char.ConvertFromUtf32(0x1F36C), "Candy"),
            new(char.ConvertFromUtf32(0x1F36D), "Lollipop"),
            new(char.ConvertFromUtf32(0x1F36E), "Custard"),
            new(char.ConvertFromUtf32(0x1F36F), "Honey Pot"),
            new(char.ConvertFromUtf32(0x1F370), "Shortcake"),
            new(char.ConvertFromUtf32(0x1F382), "Birthday Cake"),
            new(char.ConvertFromUtf32(0x1F967), "Pie"),
            new(char.ConvertFromUtf32(0x1F9C1), "Cupcake"),
            new(char.ConvertFromUtf32(0x1F375), "Teacup Without Handle"),
            new(char.ConvertFromUtf32(0x1F376), "Sake"),
            new(char.ConvertFromUtf32(0x1F377), "Wine Glass"),
            new(char.ConvertFromUtf32(0x1F378), "Cocktail Glass"),
            new(char.ConvertFromUtf32(0x1F379), "Tropical Drink"),
            new(char.ConvertFromUtf32(0x1F37A), "Beer Mug"),
            new(char.ConvertFromUtf32(0x1F37B), "Clinking Beer Mugs"),
            new(char.ConvertFromUtf32(0x1F37C), "Baby Bottle"),
            new(char.ConvertFromUtf32(0x1F37E), "Bottle with Popping Cork"),
            new(char.ConvertFromUtf32(0x1F942), "Clinking Glasses"),
            new(char.ConvertFromUtf32(0x1F943), "Tumbler Glass"),
            new(char.ConvertFromUtf32(0x1F95B), "Glass of Milk"),
            new(char.ConvertFromUtf32(0x1F964), "Cup with Straw"),
            new(char.ConvertFromUtf32(0x1F9C3), "Beverage Box"),
            new(char.ConvertFromUtf32(0x1F9C9), "Mate"),
            new(char.ConvertFromUtf32(0x1F9CA), "Ice"),
            new(char.ConvertFromUtf32(0x1F9CB), "Bubble Tea"),
            new(char.ConvertFromUtf32(0x1FAD6), "Teapot"),
            new("\u2615", "Hot Beverage"),
            new(char.ConvertFromUtf32(0x1F374), "Fork and Knife"),
            new(char.ConvertFromUtf32(0x1F37D) + emojiStyle.Value, "Fork and Knife with Plate"),
            new(char.ConvertFromUtf32(0x1F3FA), "Amphora"),
            new(char.ConvertFromUtf32(0x1F52A), "Kitchen Knife"),
            new(char.ConvertFromUtf32(0x1F944), "Spoon"),
            new(char.ConvertFromUtf32(0x1F962), "Chopsticks"));

        public static readonly Emoji motorcycle = new(char.ConvertFromUtf32(0x1F3CD) + emojiStyle.Value, "Motorcycle");
        public static readonly Emoji racingCar = new(char.ConvertFromUtf32(0x1F3CE) + emojiStyle.Value, "Racing Car");
        public static readonly Emoji seat = new(char.ConvertFromUtf32(0x1F4BA), "Seat");
        public static readonly Emoji helicopter = new(char.ConvertFromUtf32(0x1F681), "Helicopter");
        public static readonly Emoji locomotive = new(char.ConvertFromUtf32(0x1F682), "Locomotive");
        public static readonly Emoji railwayCar = new(char.ConvertFromUtf32(0x1F683), "Railway Car");
        public static readonly Emoji highspeedTrain = new(char.ConvertFromUtf32(0x1F684), "High-Speed Train");
        public static readonly Emoji bulletTrain = new(char.ConvertFromUtf32(0x1F685), "Bullet Train");
        public static readonly Emoji train = new(char.ConvertFromUtf32(0x1F686), "Train");
        public static readonly Emoji metro = new(char.ConvertFromUtf32(0x1F687), "Metro");
        public static readonly Emoji lightRail = new(char.ConvertFromUtf32(0x1F688), "Light Rail");
        public static readonly Emoji station = new(char.ConvertFromUtf32(0x1F689), "Station");
        public static readonly Emoji tram = new(char.ConvertFromUtf32(0x1F68A), "Tram");
        public static readonly Emoji tramCar = new(char.ConvertFromUtf32(0x1F68B), "Tram Car");
        public static readonly Emoji bus = new(char.ConvertFromUtf32(0x1F68C), "Bus");
        public static readonly Emoji oncomingBus = new(char.ConvertFromUtf32(0x1F68D), "Oncoming Bus");
        public static readonly Emoji trolleyBus = new(char.ConvertFromUtf32(0x1F68E), "Trolleybus");
        public static readonly Emoji busStop = new(char.ConvertFromUtf32(0x1F68F), "Bus Stop");
        public static readonly Emoji miniBus = new(char.ConvertFromUtf32(0x1F690), "Minibus");
        public static readonly Emoji ambulance = new(char.ConvertFromUtf32(0x1F691), "Ambulance");
        public static readonly Emoji policeCar = new(char.ConvertFromUtf32(0x1F693), "Police Car");
        public static readonly Emoji oncomingPoliceCar = new(char.ConvertFromUtf32(0x1F694), "Oncoming Police Car");
        public static readonly Emoji taxi = new(char.ConvertFromUtf32(0x1F695), "Taxi");
        public static readonly Emoji oncomingTaxi = new(char.ConvertFromUtf32(0x1F696), "Oncoming Taxi");
        public static readonly Emoji automobile = new(char.ConvertFromUtf32(0x1F697), "Automobile");
        public static readonly Emoji oncomingAutomobile = new(char.ConvertFromUtf32(0x1F698), "Oncoming Automobile");
        public static readonly Emoji sportUtilityVehicle = new(char.ConvertFromUtf32(0x1F699), "Sport Utility Vehicle");
        public static readonly Emoji deliveryTruck = new(char.ConvertFromUtf32(0x1F69A), "Delivery Truck");
        public static readonly Emoji articulatedLorry = new(char.ConvertFromUtf32(0x1F69B), "Articulated Lorry");
        public static readonly Emoji tractor = new(char.ConvertFromUtf32(0x1F69C), "Tractor");
        public static readonly Emoji monorail = new(char.ConvertFromUtf32(0x1F69D), "Monorail");
        public static readonly Emoji mountainRailway = new(char.ConvertFromUtf32(0x1F69E), "Mountain Railway");
        public static readonly Emoji suspensionRailway = new(char.ConvertFromUtf32(0x1F69F), "Suspension Railway");
        public static readonly Emoji mountainCableway = new(char.ConvertFromUtf32(0x1F6A0), "Mountain Cableway");
        public static readonly Emoji aerialTramway = new(char.ConvertFromUtf32(0x1F6A1), "Aerial Tramway");
        public static readonly Emoji ship = new(char.ConvertFromUtf32(0x1F6A2), "Ship");
        public static readonly Emoji speedBoat = new(char.ConvertFromUtf32(0x1F6A4), "Speedboat");
        public static readonly Emoji horizontalTrafficLight = new(char.ConvertFromUtf32(0x1F6A5), "Horizontal Traffic Light");
        public static readonly Emoji verticalTrafficLight = new(char.ConvertFromUtf32(0x1F6A6), "Vertical Traffic Light");
        public static readonly Emoji construction = new(char.ConvertFromUtf32(0x1F6A7), "Construction");
        public static readonly Emoji policeCarLight = new(char.ConvertFromUtf32(0x1F6A8), "Police Car Light");
        public static readonly Emoji bicycle = new(char.ConvertFromUtf32(0x1F6B2), "Bicycle");
        public static readonly Emoji stopSign = new(char.ConvertFromUtf32(0x1F6D1), "Stop Sign");
        public static readonly Emoji oilDrum = new(char.ConvertFromUtf32(0x1F6E2) + emojiStyle.Value, "Oil Drum");
        public static readonly Emoji motorway = new(char.ConvertFromUtf32(0x1F6E3) + emojiStyle.Value, "Motorway");
        public static readonly Emoji railwayTrack = new(char.ConvertFromUtf32(0x1F6E4) + emojiStyle.Value, "Railway Track");
        public static readonly Emoji motorBoat = new(char.ConvertFromUtf32(0x1F6E5) + emojiStyle.Value, "Motor Boat");
        public static readonly Emoji smallAirplane = new(char.ConvertFromUtf32(0x1F6E9) + emojiStyle.Value, "Small Airplane");
        public static readonly Emoji airplaneDeparture = new(char.ConvertFromUtf32(0x1F6EB), "Airplane Departure");
        public static readonly Emoji airplaneArrival = new(char.ConvertFromUtf32(0x1F6EC), "Airplane Arrival");
        public static readonly Emoji satellite = new(char.ConvertFromUtf32(0x1F6F0) + emojiStyle.Value, "Satellite");
        public static readonly Emoji passengerShip = new(char.ConvertFromUtf32(0x1F6F3) + emojiStyle.Value, "Passenger Ship");
        public static readonly Emoji kickScooter = new(char.ConvertFromUtf32(0x1F6F4), "Kick Scooter");
        public static readonly Emoji motorScooter = new(char.ConvertFromUtf32(0x1F6F5), "Motor Scooter");
        public static readonly Emoji canoe = new(char.ConvertFromUtf32(0x1F6F6), "Canoe");
        public static readonly Emoji flyingSaucer = new(char.ConvertFromUtf32(0x1F6F8), "Flying Saucer");
        public static readonly Emoji skateboard = new(char.ConvertFromUtf32(0x1F6F9), "Skateboard");
        public static readonly Emoji autoRickshaw = new(char.ConvertFromUtf32(0x1F6FA), "Auto Rickshaw");
        public static readonly Emoji pickupTruck = new(char.ConvertFromUtf32(0x1F6FB), "Pickup Truck");
        public static readonly Emoji rollerSkate = new(char.ConvertFromUtf32(0x1F6FC), "Roller Skate");
        public static readonly Emoji parachute = new(char.ConvertFromUtf32(0x1FA82), "Parachute");
        public static readonly Emoji anchor = new("\u2693", "Anchor");
        public static readonly Emoji ferry = new("\u26F4" + emojiStyle.Value, "Ferry");
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
        private static readonly Emoji bloodTypeButtonA = new(char.ConvertFromUtf32(0x1F170), "A Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonB = new(char.ConvertFromUtf32(0x1F171), "B Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonO = new(char.ConvertFromUtf32(0x1F17E), "O Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonAB = new(char.ConvertFromUtf32(0x1F18E), "AB Button (Blood Type)");
        public static readonly EmojiGroup bloodTypes = new(
            "Blood Types", "Blood types",
            bloodTypeButtonA,
            bloodTypeButtonB,
            bloodTypeButtonO,
            bloodTypeButtonAB);

        public static readonly Emoji regionalIndicatorSymbolLetterA = new(char.ConvertFromUtf32(0x1F1E6), "Regional Indicator Symbol Letter A");
        public static readonly Emoji regionalIndicatorSymbolLetterB = new(char.ConvertFromUtf32(0x1F1E7), "Regional Indicator Symbol Letter B");
        public static readonly Emoji regionalIndicatorSymbolLetterC = new(char.ConvertFromUtf32(0x1F1E8), "Regional Indicator Symbol Letter C");
        public static readonly Emoji regionalIndicatorSymbolLetterD = new(char.ConvertFromUtf32(0x1F1E9), "Regional Indicator Symbol Letter D");
        public static readonly Emoji regionalIndicatorSymbolLetterE = new(char.ConvertFromUtf32(0x1F1EA), "Regional Indicator Symbol Letter E");
        public static readonly Emoji regionalIndicatorSymbolLetterF = new(char.ConvertFromUtf32(0x1F1EB), "Regional Indicator Symbol Letter F");
        public static readonly Emoji regionalIndicatorSymbolLetterG = new(char.ConvertFromUtf32(0x1F1EC), "Regional Indicator Symbol Letter G");
        public static readonly Emoji regionalIndicatorSymbolLetterH = new(char.ConvertFromUtf32(0x1F1ED), "Regional Indicator Symbol Letter H");
        public static readonly Emoji regionalIndicatorSymbolLetterI = new(char.ConvertFromUtf32(0x1F1EE), "Regional Indicator Symbol Letter I");
        public static readonly Emoji regionalIndicatorSymbolLetterJ = new(char.ConvertFromUtf32(0x1F1EF), "Regional Indicator Symbol Letter J");
        public static readonly Emoji regionalIndicatorSymbolLetterK = new(char.ConvertFromUtf32(0x1F1F0), "Regional Indicator Symbol Letter K");
        public static readonly Emoji regionalIndicatorSymbolLetterL = new(char.ConvertFromUtf32(0x1F1F1), "Regional Indicator Symbol Letter L");
        public static readonly Emoji regionalIndicatorSymbolLetterM = new(char.ConvertFromUtf32(0x1F1F2), "Regional Indicator Symbol Letter M");
        public static readonly Emoji regionalIndicatorSymbolLetterN = new(char.ConvertFromUtf32(0x1F1F3), "Regional Indicator Symbol Letter N");
        public static readonly Emoji regionalIndicatorSymbolLetterO = new(char.ConvertFromUtf32(0x1F1F4), "Regional Indicator Symbol Letter O");
        public static readonly Emoji regionalIndicatorSymbolLetterP = new(char.ConvertFromUtf32(0x1F1F5), "Regional Indicator Symbol Letter P");
        public static readonly Emoji regionalIndicatorSymbolLetterQ = new(char.ConvertFromUtf32(0x1F1F6), "Regional Indicator Symbol Letter Q");
        public static readonly Emoji regionalIndicatorSymbolLetterR = new(char.ConvertFromUtf32(0x1F1F7), "Regional Indicator Symbol Letter R");
        public static readonly Emoji regionalIndicatorSymbolLetterS = new(char.ConvertFromUtf32(0x1F1F8), "Regional Indicator Symbol Letter S");
        public static readonly Emoji regionalIndicatorSymbolLetterT = new(char.ConvertFromUtf32(0x1F1F9), "Regional Indicator Symbol Letter T");
        public static readonly Emoji regionalIndicatorSymbolLetterU = new(char.ConvertFromUtf32(0x1F1FA), "Regional Indicator Symbol Letter U");
        public static readonly Emoji regionalIndicatorSymbolLetterV = new(char.ConvertFromUtf32(0x1F1FB), "Regional Indicator Symbol Letter V");
        public static readonly Emoji regionalIndicatorSymbolLetterW = new(char.ConvertFromUtf32(0x1F1FC), "Regional Indicator Symbol Letter W");
        public static readonly Emoji regionalIndicatorSymbolLetterX = new(char.ConvertFromUtf32(0x1F1FD), "Regional Indicator Symbol Letter X");
        public static readonly Emoji regionalIndicatorSymbolLetterY = new(char.ConvertFromUtf32(0x1F1FE), "Regional Indicator Symbol Letter Y");
        public static readonly Emoji regionalIndicatorSymbolLetterZ = new(char.ConvertFromUtf32(0x1F1FF), "Regional Indicator Symbol Letter Z");
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
            new(char.ConvertFromUtf32(0x1F530), "Japanese Symbol for Beginner"),
            new(char.ConvertFromUtf32(0x1F201), "Japanese “Here” Button"),
            new(char.ConvertFromUtf32(0x1F202) + emojiStyle.Value, "Japanese “Service Charge” Button"),
            new(char.ConvertFromUtf32(0x1F21A), "Japanese “Free of Charge” Button"),
            new(char.ConvertFromUtf32(0x1F22F), "Japanese “Reserved” Button"),
            new(char.ConvertFromUtf32(0x1F232), "Japanese “Prohibited” Button"),
            new(char.ConvertFromUtf32(0x1F233), "Japanese “Vacancy” Button"),
            new(char.ConvertFromUtf32(0x1F234), "Japanese “Passing Grade” Button"),
            new(char.ConvertFromUtf32(0x1F235), "Japanese “No Vacancy” Button"),
            new(char.ConvertFromUtf32(0x1F236), "Japanese “Not Free of Charge” Button"),
            new(char.ConvertFromUtf32(0x1F237) + emojiStyle.Value, "Japanese “Monthly Amount” Button"),
            new(char.ConvertFromUtf32(0x1F238), "Japanese “Application” Button"),
            new(char.ConvertFromUtf32(0x1F239), "Japanese “Discount” Button"),
            new(char.ConvertFromUtf32(0x1F23A), "Japanese “Open for Business” Button"),
            new(char.ConvertFromUtf32(0x1F250), "Japanese “Bargain” Button"),
            new(char.ConvertFromUtf32(0x1F251), "Japanese “Acceptable” Button"),
            new("\u3297" + emojiStyle.Value, "Japanese “Congratulations” Button"),
            new("\u3299" + emojiStyle.Value, "Japanese “Secret” Button"));

        public static readonly EmojiGroup clocks = new(
            "Clocks", "Time-keeping pieces",
            new(char.ConvertFromUtf32(0x1F550), "One O’Clock"),
            new(char.ConvertFromUtf32(0x1F551), "Two O’Clock"),
            new(char.ConvertFromUtf32(0x1F552), "Three O’Clock"),
            new(char.ConvertFromUtf32(0x1F553), "Four O’Clock"),
            new(char.ConvertFromUtf32(0x1F554), "Five O’Clock"),
            new(char.ConvertFromUtf32(0x1F555), "Six O’Clock"),
            new(char.ConvertFromUtf32(0x1F556), "Seven O’Clock"),
            new(char.ConvertFromUtf32(0x1F557), "Eight O’Clock"),
            new(char.ConvertFromUtf32(0x1F558), "Nine O’Clock"),
            new(char.ConvertFromUtf32(0x1F559), "Ten O’Clock"),
            new(char.ConvertFromUtf32(0x1F55A), "Eleven O’Clock"),
            new(char.ConvertFromUtf32(0x1F55B), "Twelve O’Clock"),
            new(char.ConvertFromUtf32(0x1F55C), "One-Thirty"),
            new(char.ConvertFromUtf32(0x1F55D), "Two-Thirty"),
            new(char.ConvertFromUtf32(0x1F55E), "Three-Thirty"),
            new(char.ConvertFromUtf32(0x1F55F), "Four-Thirty"),
            new(char.ConvertFromUtf32(0x1F560), "Five-Thirty"),
            new(char.ConvertFromUtf32(0x1F561), "Six-Thirty"),
            new(char.ConvertFromUtf32(0x1F562), "Seven-Thirty"),
            new(char.ConvertFromUtf32(0x1F563), "Eight-Thirty"),
            new(char.ConvertFromUtf32(0x1F564), "Nine-Thirty"),
            new(char.ConvertFromUtf32(0x1F565), "Ten-Thirty"),
            new(char.ConvertFromUtf32(0x1F566), "Eleven-Thirty"),
            new(char.ConvertFromUtf32(0x1F567), "Twelve-Thirty"),
            new(char.ConvertFromUtf32(0x1F570) + emojiStyle.Value, "Mantelpiece Clock"),
            new("\u231A", "Watch"),
            new("\u23F0", "Alarm Clock"),
            new("\u23F1" + emojiStyle.Value, "Stopwatch"),
            new("\u23F2" + emojiStyle.Value, "Timer Clock"),
            new("\u231B", "Hourglass Done"),
            new("\u23F3", "Hourglass Not Done"));

        public static readonly Emoji clockwiseVerticalArrows = new(char.ConvertFromUtf32(0x1F503) + emojiStyle.Value, "Clockwise Vertical Arrows");
        public static readonly Emoji counterclockwiseArrowsButton = new(char.ConvertFromUtf32(0x1F504) + emojiStyle.Value, "Counterclockwise Arrows Button");
        public static readonly Emoji leftRightArrow = new("\u2194" + emojiStyle.Value, "Left-Right Arrow");
        public static readonly Emoji upDownArrow = new("\u2195" + emojiStyle.Value, "Up-Down Arrow");
        public static readonly Emoji upLeftArrow = new("\u2196" + emojiStyle.Value, "Up-Left Arrow");
        public static readonly Emoji upRightArrow = new("\u2197" + emojiStyle.Value, "Up-Right Arrow");
        public static readonly Emoji downRightArrow = new("\u2198", "Down-Right Arrow");
        public static readonly Emoji downRightArrowText = new("\u2198" + textStyle.Value, "Down-Right Arrow");
        public static readonly Emoji downRightArrowEmoji = new("\u2198" + emojiStyle.Value, "Down-Right Arrow");
        public static readonly Emoji downLeftArrow = new("\u2199" + emojiStyle.Value, "Down-Left Arrow");
        public static readonly Emoji rightArrowCurvingLeft = new("\u21A9" + emojiStyle.Value, "Right Arrow Curving Left");
        public static readonly Emoji leftArrowCurvingRight = new("\u21AA" + emojiStyle.Value, "Left Arrow Curving Right");
        public static readonly Emoji rightArrow = new("\u27A1" + emojiStyle.Value, "Right Arrow");
        public static readonly Emoji rightArrowCurvingUp = new("\u2934" + emojiStyle.Value, "Right Arrow Curving Up");
        public static readonly Emoji rightArrowCurvingDown = new("\u2935" + emojiStyle.Value, "Right Arrow Curving Down");
        public static readonly Emoji leftArrow = new("\u2B05" + emojiStyle.Value, "Left Arrow");
        public static readonly Emoji upArrow = new("\u2B06" + emojiStyle.Value, "Up Arrow");
        public static readonly Emoji downArrow = new("\u2B07" + emojiStyle.Value, "Down Arrow");
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

        public static readonly Emoji redCircle = new(char.ConvertFromUtf32(0x1F534), "Red Circle");
        public static readonly Emoji blueCircle = new(char.ConvertFromUtf32(0x1F535), "Blue Circle");
        public static readonly Emoji largeOrangeDiamond = new(char.ConvertFromUtf32(0x1F536), "Large Orange Diamond");
        public static readonly Emoji largeBlueDiamond = new(char.ConvertFromUtf32(0x1F537), "Large Blue Diamond");
        public static readonly Emoji smallOrangeDiamond = new(char.ConvertFromUtf32(0x1F538), "Small Orange Diamond");
        public static readonly Emoji smallBlueDiamond = new(char.ConvertFromUtf32(0x1F539), "Small Blue Diamond");
        public static readonly Emoji redTrianglePointedUp = new(char.ConvertFromUtf32(0x1F53A), "Red Triangle Pointed Up");
        public static readonly Emoji redTrianglePointedDown = new(char.ConvertFromUtf32(0x1F53B), "Red Triangle Pointed Down");
        public static readonly Emoji orangeCircle = new(char.ConvertFromUtf32(0x1F7E0), "Orange Circle");
        public static readonly Emoji yellowCircle = new(char.ConvertFromUtf32(0x1F7E1), "Yellow Circle");
        public static readonly Emoji greenCircle = new(char.ConvertFromUtf32(0x1F7E2), "Green Circle");
        public static readonly Emoji purpleCircle = new(char.ConvertFromUtf32(0x1F7E3), "Purple Circle");
        public static readonly Emoji brownCircle = new(char.ConvertFromUtf32(0x1F7E4), "Brown Circle");
        public static readonly Emoji hollowRedCircle = new("\u2B55", "Hollow Red Circle");
        public static readonly Emoji whiteCircle = new("\u26AA", "White Circle");
        public static readonly Emoji blackCircle = new("\u26AB", "Black Circle");
        public static readonly Emoji redSquare = new(char.ConvertFromUtf32(0x1F7E5), "Red Square");
        public static readonly Emoji blueSquare = new(char.ConvertFromUtf32(0x1F7E6), "Blue Square");
        public static readonly Emoji orangeSquare = new(char.ConvertFromUtf32(0x1F7E7), "Orange Square");
        public static readonly Emoji yellowSquare = new(char.ConvertFromUtf32(0x1F7E8), "Yellow Square");
        public static readonly Emoji greenSquare = new(char.ConvertFromUtf32(0x1F7E9), "Green Square");
        public static readonly Emoji purpleSquare = new(char.ConvertFromUtf32(0x1F7EA), "Purple Square");
        public static readonly Emoji brownSquare = new(char.ConvertFromUtf32(0x1F7EB), "Brown Square");
        public static readonly Emoji blackSquareButton = new(char.ConvertFromUtf32(0x1F532), "Black Square Button");
        public static readonly Emoji whiteSquareButton = new(char.ConvertFromUtf32(0x1F533), "White Square Button");
        public static readonly Emoji blackSmallSquare = new("\u25AA" + emojiStyle.Value, "Black Small Square");
        public static readonly Emoji whiteSmallSquare = new("\u25AB" + emojiStyle.Value, "White Small Square");
        public static readonly Emoji whiteMediumSmallSquare = new("\u25FD", "White Medium-Small Square");
        public static readonly Emoji blackMediumSmallSquare = new("\u25FE", "Black Medium-Small Square");
        public static readonly Emoji whiteMediumSquare = new("\u25FB" + emojiStyle.Value, "White Medium Square");
        public static readonly Emoji blackMediumSquare = new("\u25FC" + emojiStyle.Value, "Black Medium Square");
        public static readonly Emoji blackLargeSquare = new("\u2B1B", "Black Large Square");
        public static readonly Emoji whiteLargeSquare = new("\u2B1C", "White Large Square");
        public static readonly Emoji star = new("\u2B50", "Star");
        public static readonly Emoji diamondWithADot = new(char.ConvertFromUtf32(0x1F4A0), "Diamond with a Dot");
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

        public static readonly Emoji clearButton = new(char.ConvertFromUtf32(0x1F191), "CL Button");
        public static readonly Emoji coolButton = new(char.ConvertFromUtf32(0x1F192), "Cool Button");
        public static readonly Emoji freeButton = new(char.ConvertFromUtf32(0x1F193), "Free Button");
        public static readonly Emoji idButton = new(char.ConvertFromUtf32(0x1F194), "ID Button");
        public static readonly Emoji newButton = new(char.ConvertFromUtf32(0x1F195), "New Button");
        public static readonly Emoji ngButton = new(char.ConvertFromUtf32(0x1F196), "NG Button");
        public static readonly Emoji okButton = new(char.ConvertFromUtf32(0x1F197), "OK Button");
        public static readonly Emoji sosButton = new(char.ConvertFromUtf32(0x1F198), "SOS Button");
        public static readonly Emoji upButton = new(char.ConvertFromUtf32(0x1F199), "Up! Button");
        public static readonly Emoji vsButton = new(char.ConvertFromUtf32(0x1F19A), "Vs Button");
        public static readonly Emoji radioButton = new(char.ConvertFromUtf32(0x1F518), "Radio Button");
        public static readonly Emoji backArrow = new(char.ConvertFromUtf32(0x1F519), "Back Arrow");
        public static readonly Emoji endArrow = new(char.ConvertFromUtf32(0x1F51A), "End Arrow");
        public static readonly Emoji onArrow = new(char.ConvertFromUtf32(0x1F51B), "On! Arrow");
        public static readonly Emoji soonArrow = new(char.ConvertFromUtf32(0x1F51C), "Soon Arrow");
        public static readonly Emoji topArrow = new(char.ConvertFromUtf32(0x1F51D), "Top Arrow");
        public static readonly Emoji checkBoxWithCheck = new("\u2611" + emojiStyle.Value, "Check Box with Check");
        public static readonly Emoji inputLatinUppercase = new(char.ConvertFromUtf32(0x1F520), "Input Latin Uppercase");
        public static readonly Emoji inputLatinLowercase = new(char.ConvertFromUtf32(0x1F521), "Input Latin Lowercase");
        public static readonly Emoji inputNumbers = new(char.ConvertFromUtf32(0x1F522), "Input Numbers");
        public static readonly Emoji inputSymbols = new(char.ConvertFromUtf32(0x1F523), "Input Symbols");
        public static readonly Emoji inputLatinLetters = new(char.ConvertFromUtf32(0x1F524), "Input Latin Letters");
        public static readonly Emoji shuffleTracksButton = new(char.ConvertFromUtf32(0x1F500), "Shuffle Tracks Button");
        public static readonly Emoji repeatButton = new(char.ConvertFromUtf32(0x1F501), "Repeat Button");
        public static readonly Emoji repeatSingleButton = new(char.ConvertFromUtf32(0x1F502), "Repeat Single Button");
        public static readonly Emoji upwardsButton = new(char.ConvertFromUtf32(0x1F53C), "Upwards Button");
        public static readonly Emoji downwardsButton = new(char.ConvertFromUtf32(0x1F53D), "Downwards Button");
        public static readonly Emoji playButton = new("\u25B6" + emojiStyle.Value, "Play Button");
        public static readonly Emoji reverseButton = new("\u25C0" + emojiStyle.Value, "Reverse Button");
        public static readonly Emoji ejectButton = new("\u23CF" + emojiStyle.Value, "Eject Button");
        public static readonly Emoji fastForwardButton = new("\u23E9", "Fast-Forward Button");
        public static readonly Emoji fastReverseButton = new("\u23EA", "Fast Reverse Button");
        public static readonly Emoji fastUpButton = new("\u23EB", "Fast Up Button");
        public static readonly Emoji fastDownButton = new("\u23EC", "Fast Down Button");
        public static readonly Emoji nextTrackButton = new("\u23ED" + emojiStyle.Value, "Next Track Button");
        public static readonly Emoji lastTrackButton = new("\u23EE" + emojiStyle.Value, "Last Track Button");
        public static readonly Emoji playOrPauseButton = new("\u23EF" + emojiStyle.Value, "Play or Pause Button");
        public static readonly Emoji pauseButton = new("\u23F8" + emojiStyle.Value, "Pause Button");
        public static readonly Emoji stopButton = new("\u23F9" + emojiStyle.Value, "Stop Button");
        public static readonly Emoji recordButton = new("\u23FA" + emojiStyle.Value, "Record Button");
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
            new("0" + emojiStyle.Value, "Digit Zero"),
            new("1" + emojiStyle.Value, "Digit One"),
            new("2" + emojiStyle.Value, "Digit Two"),
            new("3" + emojiStyle.Value, "Digit Three"),
            new("4" + emojiStyle.Value, "Digit Four"),
            new("5" + emojiStyle.Value, "Digit Five"),
            new("6" + emojiStyle.Value, "Digit Six"),
            new("7" + emojiStyle.Value, "Digit Seven"),
            new("8" + emojiStyle.Value, "Digit Eight"),
            new("9" + emojiStyle.Value, "Digit Nine"),
            new("*" + emojiStyle.Value, "Asterisk"),
            new("#" + emojiStyle.Value, "Number Sign"),
            new("0" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Zero"),
            new("1" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit One"),
            new("2" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Two"),
            new("3" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Three"),
            new("4" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Four"),
            new("5" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Five"),
            new("6" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Six"),
            new("7" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Seven"),
            new("8" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Eight"),
            new("9" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Nine"),
            new("*" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Asterisk"),
            new("#" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Number Sign"),
            new(char.ConvertFromUtf32(0x1F51F), "Keycap: 10"));

        public static readonly Emoji tagPlusSign = new(char.ConvertFromUtf32(0xE002B), "Tag Plus Sign");
        public static readonly Emoji tagMinusHyphen = new(char.ConvertFromUtf32(0xE002D), "Tag Hyphen-Minus");
        public static readonly Emoji tagLatinSmallLetterA = new(char.ConvertFromUtf32(0xE0061), "Tag Latin Small Letter A");
        public static readonly Emoji tagLatinSmallLetterB = new(char.ConvertFromUtf32(0xE0062), "Tag Latin Small Letter B");
        public static readonly Emoji tagLatinSmallLetterC = new(char.ConvertFromUtf32(0xE0063), "Tag Latin Small Letter C");
        public static readonly Emoji tagLatinSmallLetterD = new(char.ConvertFromUtf32(0xE0064), "Tag Latin Small Letter D");
        public static readonly Emoji tagLatinSmallLetterE = new(char.ConvertFromUtf32(0xE0065), "Tag Latin Small Letter E");
        public static readonly Emoji tagLatinSmallLetterF = new(char.ConvertFromUtf32(0xE0066), "Tag Latin Small Letter F");
        public static readonly Emoji tagLatinSmallLetterG = new(char.ConvertFromUtf32(0xE0067), "Tag Latin Small Letter G");
        public static readonly Emoji tagLatinSmallLetterH = new(char.ConvertFromUtf32(0xE0068), "Tag Latin Small Letter H");
        public static readonly Emoji tagLatinSmallLetterI = new(char.ConvertFromUtf32(0xE0069), "Tag Latin Small Letter I");
        public static readonly Emoji tagLatinSmallLetterJ = new(char.ConvertFromUtf32(0xE006A), "Tag Latin Small Letter J");
        public static readonly Emoji tagLatinSmallLetterK = new(char.ConvertFromUtf32(0xE006B), "Tag Latin Small Letter K");
        public static readonly Emoji tagLatinSmallLetterL = new(char.ConvertFromUtf32(0xE006C), "Tag Latin Small Letter L");
        public static readonly Emoji tagLatinSmallLetterM = new(char.ConvertFromUtf32(0xE006D), "Tag Latin Small Letter M");
        public static readonly Emoji tagLatinSmallLetterN = new(char.ConvertFromUtf32(0xE006E), "Tag Latin Small Letter N");
        public static readonly Emoji tagLatinSmallLetterO = new(char.ConvertFromUtf32(0xE006F), "Tag Latin Small Letter O");
        public static readonly Emoji tagLatinSmallLetterP = new(char.ConvertFromUtf32(0xE0070), "Tag Latin Small Letter P");
        public static readonly Emoji tagLatinSmallLetterQ = new(char.ConvertFromUtf32(0xE0071), "Tag Latin Small Letter Q");
        public static readonly Emoji tagLatinSmallLetterR = new(char.ConvertFromUtf32(0xE0072), "Tag Latin Small Letter R");
        public static readonly Emoji tagLatinSmallLetterS = new(char.ConvertFromUtf32(0xE0073), "Tag Latin Small Letter S");
        public static readonly Emoji tagLatinSmallLetterT = new(char.ConvertFromUtf32(0xE0074), "Tag Latin Small Letter T");
        public static readonly Emoji tagLatinSmallLetterU = new(char.ConvertFromUtf32(0xE0075), "Tag Latin Small Letter U");
        public static readonly Emoji tagLatinSmallLetterV = new(char.ConvertFromUtf32(0xE0076), "Tag Latin Small Letter V");
        public static readonly Emoji tagLatinSmallLetterW = new(char.ConvertFromUtf32(0xE0077), "Tag Latin Small Letter W");
        public static readonly Emoji tagLatinSmallLetterX = new(char.ConvertFromUtf32(0xE0078), "Tag Latin Small Letter X");
        public static readonly Emoji tagLatinSmallLetterY = new(char.ConvertFromUtf32(0xE0079), "Tag Latin Small Letter Y");
        public static readonly Emoji tagLatinSmallLetterZ = new(char.ConvertFromUtf32(0xE007A), "Tag Latin Small Letter Z");
        public static readonly Emoji cancelTag = new(char.ConvertFromUtf32(0xE007F), "Cancel Tag");
        public static readonly EmojiGroup tags = new(
            "Tags", "Tags",
            new(char.ConvertFromUtf32(0xE0020), "Tag Space"),
            new(char.ConvertFromUtf32(0xE0021), "Tag Exclamation Mark"),
            new(char.ConvertFromUtf32(0xE0022), "Tag Quotation Mark"),
            new(char.ConvertFromUtf32(0xE0023), "Tag Number Sign"),
            new(char.ConvertFromUtf32(0xE0024), "Tag Dollar Sign"),
            new(char.ConvertFromUtf32(0xE0025), "Tag Percent Sign"),
            new(char.ConvertFromUtf32(0xE0026), "Tag Ampersand"),
            new(char.ConvertFromUtf32(0xE0027), "Tag Apostrophe"),
            new(char.ConvertFromUtf32(0xE0028), "Tag Left Parenthesis"),
            new(char.ConvertFromUtf32(0xE0029), "Tag Right Parenthesis"),
            new(char.ConvertFromUtf32(0xE002A), "Tag Asterisk"),
            tagPlusSign,
            new(char.ConvertFromUtf32(0xE002C), "Tag Comma"),
            tagMinusHyphen,
            new(char.ConvertFromUtf32(0xE002E), "Tag Full Stop"),
            new(char.ConvertFromUtf32(0xE002F), "Tag Solidus"),
            new(char.ConvertFromUtf32(0xE0030), "Tag Digit Zero"),
            new(char.ConvertFromUtf32(0xE0031), "Tag Digit One"),
            new(char.ConvertFromUtf32(0xE0032), "Tag Digit Two"),
            new(char.ConvertFromUtf32(0xE0033), "Tag Digit Three"),
            new(char.ConvertFromUtf32(0xE0034), "Tag Digit Four"),
            new(char.ConvertFromUtf32(0xE0035), "Tag Digit Five"),
            new(char.ConvertFromUtf32(0xE0036), "Tag Digit Six"),
            new(char.ConvertFromUtf32(0xE0037), "Tag Digit Seven"),
            new(char.ConvertFromUtf32(0xE0038), "Tag Digit Eight"),
            new(char.ConvertFromUtf32(0xE0039), "Tag Digit Nine"),
            new(char.ConvertFromUtf32(0xE003A), "Tag Colon"),
            new(char.ConvertFromUtf32(0xE003B), "Tag Semicolon"),
            new(char.ConvertFromUtf32(0xE003C), "Tag Less-Than Sign"),
            new(char.ConvertFromUtf32(0xE003D), "Tag Equals Sign"),
            new(char.ConvertFromUtf32(0xE003E), "Tag Greater-Than Sign"),
            new(char.ConvertFromUtf32(0xE003F), "Tag Question Mark"),
            new(char.ConvertFromUtf32(0xE0040), "Tag Commercial at"),
            new(char.ConvertFromUtf32(0xE0041), "Tag Latin Capital Letter a"),
            new(char.ConvertFromUtf32(0xE0042), "Tag Latin Capital Letter B"),
            new(char.ConvertFromUtf32(0xE0043), "Tag Latin Capital Letter C"),
            new(char.ConvertFromUtf32(0xE0044), "Tag Latin Capital Letter D"),
            new(char.ConvertFromUtf32(0xE0045), "Tag Latin Capital Letter E"),
            new(char.ConvertFromUtf32(0xE0046), "Tag Latin Capital Letter F"),
            new(char.ConvertFromUtf32(0xE0047), "Tag Latin Capital Letter G"),
            new(char.ConvertFromUtf32(0xE0048), "Tag Latin Capital Letter H"),
            new(char.ConvertFromUtf32(0xE0049), "Tag Latin Capital Letter I"),
            new(char.ConvertFromUtf32(0xE004A), "Tag Latin Capital Letter J"),
            new(char.ConvertFromUtf32(0xE004B), "Tag Latin Capital Letter K"),
            new(char.ConvertFromUtf32(0xE004C), "Tag Latin Capital Letter L"),
            new(char.ConvertFromUtf32(0xE004D), "Tag Latin Capital Letter M"),
            new(char.ConvertFromUtf32(0xE004E), "Tag Latin Capital Letter N"),
            new(char.ConvertFromUtf32(0xE004F), "Tag Latin Capital Letter O"),
            new(char.ConvertFromUtf32(0xE0050), "Tag Latin Capital Letter P"),
            new(char.ConvertFromUtf32(0xE0051), "Tag Latin Capital Letter Q"),
            new(char.ConvertFromUtf32(0xE0052), "Tag Latin Capital Letter R"),
            new(char.ConvertFromUtf32(0xE0053), "Tag Latin Capital Letter S"),
            new(char.ConvertFromUtf32(0xE0054), "Tag Latin Capital Letter T"),
            new(char.ConvertFromUtf32(0xE0055), "Tag Latin Capital Letter U"),
            new(char.ConvertFromUtf32(0xE0056), "Tag Latin Capital Letter V"),
            new(char.ConvertFromUtf32(0xE0057), "Tag Latin Capital Letter W"),
            new(char.ConvertFromUtf32(0xE0058), "Tag Latin Capital Letter X"),
            new(char.ConvertFromUtf32(0xE0059), "Tag Latin Capital Letter Y"),
            new(char.ConvertFromUtf32(0xE005A), "Tag Latin Capital Letter Z"),
            new(char.ConvertFromUtf32(0xE005B), "Tag Left Square Bracket"),
            new(char.ConvertFromUtf32(0xE005C), "Tag Reverse Solidus"),
            new(char.ConvertFromUtf32(0xE005D), "Tag Right Square Bracket"),
            new(char.ConvertFromUtf32(0xE005E), "Tag Circumflex Accent"),
            new(char.ConvertFromUtf32(0xE005F), "Tag Low Line"),
            new(char.ConvertFromUtf32(0xE0060), "Tag Grave Accent"),
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
            new(char.ConvertFromUtf32(0xE007B), "Tag Left Curly Bracket"),
            new(char.ConvertFromUtf32(0xE007C), "Tag Vertical Line"),
            new(char.ConvertFromUtf32(0xE007D), "Tag Right Curly Bracket"),
            new(char.ConvertFromUtf32(0xE007E), "Tag Tilde"),
            cancelTag);

        public static readonly EmojiGroup math = new(
            "Math", "Math",
            new("\u2716" + emojiStyle.Value, "Multiply"),
            new("\u2795", "Plus"),
            new("\u2796", "Minus"),
            new("\u2797", "Divide"));

        public static readonly EmojiGroup games = new(
            "Games", "Games",
            new("\u2660" + emojiStyle.Value, "Spade Suit"),
            new("\u2663" + emojiStyle.Value, "Club Suit"),
            new("\u2665" + emojiStyle.Value, "Heart Suit"),
            new("\u2666" + emojiStyle.Value, "Diamond Suit"),
            new(char.ConvertFromUtf32(0x1F004), "Mahjong Red Dragon"),
            new(char.ConvertFromUtf32(0x1F0CF), "Joker"),
            new(char.ConvertFromUtf32(0x1F3AF), "Direct Hit"),
            new(char.ConvertFromUtf32(0x1F3B0), "Slot Machine"),
            new(char.ConvertFromUtf32(0x1F3B1), "Pool 8 Ball"),
            new(char.ConvertFromUtf32(0x1F3B2), "Game Die"),
            new(char.ConvertFromUtf32(0x1F3B3), "Bowling"),
            new(char.ConvertFromUtf32(0x1F3B4), "Flower Playing Cards"),
            new(char.ConvertFromUtf32(0x1F9E9), "Puzzle Piece"),
            new("\u265F" + emojiStyle.Value, "Chess Pawn"),
            new(char.ConvertFromUtf32(0x1FA80), "Yo-Yo"),
            new(char.ConvertFromUtf32(0x1FA83), "Boomerang"),
            new(char.ConvertFromUtf32(0x1FA86), "Nesting Dolls"),
            new(char.ConvertFromUtf32(0x1FA81), "Kite"));

        public static readonly EmojiGroup sportsEquipment = new(
            "Sports Equipment", "Sports equipment",
            new(char.ConvertFromUtf32(0x1F3BD), "Running Shirt"),
            new(char.ConvertFromUtf32(0x1F3BE), "Tennis"),
            new(char.ConvertFromUtf32(0x1F3BF), "Skis"),
            new(char.ConvertFromUtf32(0x1F3C0), "Basketball"),
            new(char.ConvertFromUtf32(0x1F3C5), "Sports Medal"),
            new(char.ConvertFromUtf32(0x1F3C6), "Trophy"),
            new(char.ConvertFromUtf32(0x1F3C8), "American Football"),
            new(char.ConvertFromUtf32(0x1F3C9), "Rugby Football"),
            new(char.ConvertFromUtf32(0x1F3CF), "Cricket Game"),
            new(char.ConvertFromUtf32(0x1F3D0), "Volleyball"),
            new(char.ConvertFromUtf32(0x1F3D1), "Field Hockey"),
            new(char.ConvertFromUtf32(0x1F3D2), "Ice Hockey"),
            new(char.ConvertFromUtf32(0x1F3D3), "Ping Pong"),
            new(char.ConvertFromUtf32(0x1F3F8), "Badminton"),
            new(char.ConvertFromUtf32(0x1F6F7), "Sled"),
            new(char.ConvertFromUtf32(0x1F945), "Goal Net"),
            new(char.ConvertFromUtf32(0x1F947), "1st Place Medal"),
            new(char.ConvertFromUtf32(0x1F948), "2nd Place Medal"),
            new(char.ConvertFromUtf32(0x1F949), "3rd Place Medal"),
            new(char.ConvertFromUtf32(0x1F94A), "Boxing Glove"),
            new(char.ConvertFromUtf32(0x1F94C), "Curling Stone"),
            new(char.ConvertFromUtf32(0x1F94D), "Lacrosse"),
            new(char.ConvertFromUtf32(0x1F94E), "Softball"),
            new(char.ConvertFromUtf32(0x1F94F), "Flying Disc"),
            new("\u26BD", "Soccer Ball"),
            new("\u26BE", "Baseball"),
            new("\u26F8" + emojiStyle.Value, "Ice Skate"));

        public static readonly Emoji safetyVest = new(char.ConvertFromUtf32(0x1F9BA), "Safety Vest");

        public static readonly EmojiGroup clothing = new(
            "Clothing", "Clothing",
            new(char.ConvertFromUtf32(0x1F3A9), "Top Hat"),
            new(char.ConvertFromUtf32(0x1F93F), "Diving Mask"),
            new(char.ConvertFromUtf32(0x1F452), "Woman’s Hat"),
            new(char.ConvertFromUtf32(0x1F453), "Glasses"),
            new(char.ConvertFromUtf32(0x1F576) + emojiStyle.Value, "Sunglasses"),
            new(char.ConvertFromUtf32(0x1F454), "Necktie"),
            new(char.ConvertFromUtf32(0x1F455), "T-Shirt"),
            new(char.ConvertFromUtf32(0x1F456), "Jeans"),
            new(char.ConvertFromUtf32(0x1F457), "Dress"),
            new(char.ConvertFromUtf32(0x1F458), "Kimono"),
            new(char.ConvertFromUtf32(0x1F459), "Bikini"),
            new(char.ConvertFromUtf32(0x1F45A), "Woman’s Clothes"),
            new(char.ConvertFromUtf32(0x1F45B), "Purse"),
            new(char.ConvertFromUtf32(0x1F45C), "Handbag"),
            new(char.ConvertFromUtf32(0x1F45D), "Clutch Bag"),
            new(char.ConvertFromUtf32(0x1F45E), "Man’s Shoe"),
            new(char.ConvertFromUtf32(0x1F45F), "Running Shoe"),
            new(char.ConvertFromUtf32(0x1F460), "High-Heeled Shoe"),
            new(char.ConvertFromUtf32(0x1F461), "Woman’s Sandal"),
            new(char.ConvertFromUtf32(0x1F462), "Woman’s Boot"),
            new(char.ConvertFromUtf32(0x1F94B), "Martial Arts Uniform"),
            new(char.ConvertFromUtf32(0x1F97B), "Sari"),
            new(char.ConvertFromUtf32(0x1F97C), "Lab Coat"),
            new(char.ConvertFromUtf32(0x1F97D), "Goggles"),
            new(char.ConvertFromUtf32(0x1F97E), "Hiking Boot"),
            new(char.ConvertFromUtf32(0x1F97F), "Flat Shoe"),
            whiteCane,
            safetyVest,
            new(char.ConvertFromUtf32(0x1F9E2), "Billed Cap"),
            new(char.ConvertFromUtf32(0x1F9E3), "Scarf"),
            new(char.ConvertFromUtf32(0x1F9E4), "Gloves"),
            new(char.ConvertFromUtf32(0x1F9E5), "Coat"),
            new(char.ConvertFromUtf32(0x1F9E6), "Socks"),
            new(char.ConvertFromUtf32(0x1F9FF), "Nazar Amulet"),
            new(char.ConvertFromUtf32(0x1FA70), "Ballet Shoes"),
            new(char.ConvertFromUtf32(0x1FA71), "One-Piece Swimsuit"),
            new(char.ConvertFromUtf32(0x1FA72), "Briefs"),
            new(char.ConvertFromUtf32(0x1FA73), "Shorts"));

        public static readonly EmojiGroup town = new(
            "Town", "Town",
            new(char.ConvertFromUtf32(0x1F3D7) + emojiStyle.Value, "Building Construction"),
            new(char.ConvertFromUtf32(0x1F3D8) + emojiStyle.Value, "Houses"),
            new(char.ConvertFromUtf32(0x1F3D9) + emojiStyle.Value, "Cityscape"),
            new(char.ConvertFromUtf32(0x1F3DA) + emojiStyle.Value, "Derelict House"),
            new(char.ConvertFromUtf32(0x1F3DB) + emojiStyle.Value, "Classical Building"),
            new(char.ConvertFromUtf32(0x1F3DC) + emojiStyle.Value, "Desert"),
            new(char.ConvertFromUtf32(0x1F3DD) + emojiStyle.Value, "Desert Island"),
            new(char.ConvertFromUtf32(0x1F3DE) + emojiStyle.Value, "National Park"),
            new(char.ConvertFromUtf32(0x1F3DF) + emojiStyle.Value, "Stadium"),
            new(char.ConvertFromUtf32(0x1F3E0), "House"),
            new(char.ConvertFromUtf32(0x1F3E1), "House with Garden"),
            new(char.ConvertFromUtf32(0x1F3E2), "Office Building"),
            new(char.ConvertFromUtf32(0x1F3E3), "Japanese Post Office"),
            new(char.ConvertFromUtf32(0x1F3E4), "Post Office"),
            new(char.ConvertFromUtf32(0x1F3E5), "Hospital"),
            new(char.ConvertFromUtf32(0x1F3E6), "Bank"),
            new(char.ConvertFromUtf32(0x1F3E7), "ATM Sign"),
            new(char.ConvertFromUtf32(0x1F3E8), "Hotel"),
            new(char.ConvertFromUtf32(0x1F3E9), "Love Hotel"),
            new(char.ConvertFromUtf32(0x1F3EA), "Convenience Store"),
            school,
            new(char.ConvertFromUtf32(0x1F3EC), "Department Store"),
            factory,
            new(char.ConvertFromUtf32(0x1F309), "Bridge at Night"),
            new("\u26F2", "Fountain"),
            new(char.ConvertFromUtf32(0x1F6CD) + emojiStyle.Value, "Shopping Bags"),
            new(char.ConvertFromUtf32(0x1F9FE), "Receipt"),
            new(char.ConvertFromUtf32(0x1F6D2), "Shopping Cart"),
            new(char.ConvertFromUtf32(0x1F488), "Barber Pole"),
            new(char.ConvertFromUtf32(0x1F492), "Wedding"),
            new(char.ConvertFromUtf32(0x1F5F3) + emojiStyle.Value, "Ballot Box with Ballot"));

        public static readonly EmojiGroup music = new(
            "Music", "Music",
            new(char.ConvertFromUtf32(0x1F3BC), "Musical Score"),
            new(char.ConvertFromUtf32(0x1F3B6), "Musical Notes"),
            new(char.ConvertFromUtf32(0x1F3B5), "Musical Note"),
            new(char.ConvertFromUtf32(0x1F3B7), "Saxophone"),
            new(char.ConvertFromUtf32(0x1F3B8), "Guitar"),
            new(char.ConvertFromUtf32(0x1F3B9), "Musical Keyboard"),
            new(char.ConvertFromUtf32(0x1F3BA), "Trumpet"),
            new(char.ConvertFromUtf32(0x1F3BB), "Violin"),
            new(char.ConvertFromUtf32(0x1F941), "Drum"),
            new(char.ConvertFromUtf32(0x1FA97), "Accordion"),
            new(char.ConvertFromUtf32(0x1FA98), "Long Drum"),
            new(char.ConvertFromUtf32(0x1FA95), "Banjo"));

        public static readonly Emoji snowflake = new("\u2744" + emojiStyle.Value, "Snowflake");
        public static readonly Emoji rainbow = new(char.ConvertFromUtf32(0x1F308), "Rainbow");

        public static readonly EmojiGroup weather = new(
            "Weather", "Weather",
            new(char.ConvertFromUtf32(0x1F304), "Sunrise Over Mountains"),
            new(char.ConvertFromUtf32(0x1F305), "Sunrise"),
            new(char.ConvertFromUtf32(0x1F306), "Cityscape at Dusk"),
            new(char.ConvertFromUtf32(0x1F307), "Sunset"),
            new(char.ConvertFromUtf32(0x1F303), "Night with Stars"),
            new(char.ConvertFromUtf32(0x1F302), "Closed Umbrella"),
            new("\u2602" + emojiStyle.Value, "Umbrella"),
            new("\u2614" + emojiStyle.Value, "Umbrella with Rain Drops"),
            new("\u2603" + emojiStyle.Value, "Snowman"),
            new("\u26C4", "Snowman Without Snow"),
            new("\u2600" + emojiStyle.Value, "Sun"),
            new("\u2601" + emojiStyle.Value, "Cloud"),
            new(char.ConvertFromUtf32(0x1F324) + emojiStyle.Value, "Sun Behind Small Cloud"),
            new("\u26C5", "Sun Behind Cloud"),
            new(char.ConvertFromUtf32(0x1F325) + emojiStyle.Value, "Sun Behind Large Cloud"),
            new(char.ConvertFromUtf32(0x1F326) + emojiStyle.Value, "Sun Behind Rain Cloud"),
            new(char.ConvertFromUtf32(0x1F327) + emojiStyle.Value, "Cloud with Rain"),
            new(char.ConvertFromUtf32(0x1F328) + emojiStyle.Value, "Cloud with Snow"),
            new(char.ConvertFromUtf32(0x1F329) + emojiStyle.Value, "Cloud with Lightning"),
            new("\u26C8" + emojiStyle.Value, "Cloud with Lightning and Rain"),
            snowflake,
            new(char.ConvertFromUtf32(0x1F300), "Cyclone"),
            new(char.ConvertFromUtf32(0x1F32A) + emojiStyle.Value, "Tornado"),
            new(char.ConvertFromUtf32(0x1F32C) + emojiStyle.Value, "Wind Face"),
            new(char.ConvertFromUtf32(0x1F30A), "Water Wave"),
            new(char.ConvertFromUtf32(0x1F32B) + emojiStyle.Value, "Fog"),
            new(char.ConvertFromUtf32(0x1F301), "Foggy"),
            rainbow,
            new(char.ConvertFromUtf32(0x1F321) + emojiStyle.Value, "Thermometer"));

        public static readonly EmojiGroup astro = new(
            "Astronomy", "Astronomy",
            new(char.ConvertFromUtf32(0x1F30C), "Milky Way"),
            new(char.ConvertFromUtf32(0x1F30D), "Globe Showing Europe-Africa"),
            new(char.ConvertFromUtf32(0x1F30E), "Globe Showing Americas"),
            new(char.ConvertFromUtf32(0x1F30F), "Globe Showing Asia-Australia"),
            new(char.ConvertFromUtf32(0x1F310), "Globe with Meridians"),
            new(char.ConvertFromUtf32(0x1F311), "New Moon"),
            new(char.ConvertFromUtf32(0x1F312), "Waxing Crescent Moon"),
            new(char.ConvertFromUtf32(0x1F313), "First Quarter Moon"),
            new(char.ConvertFromUtf32(0x1F314), "Waxing Gibbous Moon"),
            new(char.ConvertFromUtf32(0x1F315), "Full Moon"),
            new(char.ConvertFromUtf32(0x1F316), "Waning Gibbous Moon"),
            new(char.ConvertFromUtf32(0x1F317), "Last Quarter Moon"),
            new(char.ConvertFromUtf32(0x1F318), "Waning Crescent Moon"),
            new(char.ConvertFromUtf32(0x1F319), "Crescent Moon"),
            new(char.ConvertFromUtf32(0x1F31A), "New Moon Face"),
            new(char.ConvertFromUtf32(0x1F31B), "First Quarter Moon Face"),
            new(char.ConvertFromUtf32(0x1F31C), "Last Quarter Moon Face"),
            new(char.ConvertFromUtf32(0x1F31D), "Full Moon Face"),
            new(char.ConvertFromUtf32(0x1F31E), "Sun with Face"),
            new(char.ConvertFromUtf32(0x1F31F), "Glowing Star"),
            new(char.ConvertFromUtf32(0x1F320), "Shooting Star"),
            new("\u2604" + emojiStyle.Value, "Comet"),
            new(char.ConvertFromUtf32(0x1FA90), "Ringed Planet"));

        public static readonly EmojiGroup finance = new(
            "Finance", "Finance",
            new(char.ConvertFromUtf32(0x1F4B0), "Money Bag"),
            new(char.ConvertFromUtf32(0x1F4B1), "Currency Exchange"),
            new(char.ConvertFromUtf32(0x1F4B2), "Heavy Dollar Sign"),
            new(char.ConvertFromUtf32(0x1F4B3), "Credit Card"),
            new(char.ConvertFromUtf32(0x1F4B4), "Yen Banknote"),
            new(char.ConvertFromUtf32(0x1F4B5), "Dollar Banknote"),
            new(char.ConvertFromUtf32(0x1F4B6), "Euro Banknote"),
            new(char.ConvertFromUtf32(0x1F4B7), "Pound Banknote"),
            new(char.ConvertFromUtf32(0x1F4B8), "Money with Wings"),
            new(char.ConvertFromUtf32(0x1FA99), "Coin"),
            new(char.ConvertFromUtf32(0x1F4B9), "Chart Increasing with Yen"));

        public static readonly EmojiGroup writing = new(
            "Writing", "Writing",
            new(char.ConvertFromUtf32(0x1F58A) + emojiStyle.Value, "Pen"),
            new(char.ConvertFromUtf32(0x1F58B) + emojiStyle.Value, "Fountain Pen"),
            new(char.ConvertFromUtf32(0x1F58C) + emojiStyle.Value, "Paintbrush"),
            new(char.ConvertFromUtf32(0x1F58D) + emojiStyle.Value, "Crayon"),
            new("\u270F" + emojiStyle.Value, "Pencil"),
            new("\u2712" + emojiStyle.Value, "Black Nib"));

        public static readonly Emoji alembic = new("\u2697" + emojiStyle.Value, "Alembic");
        public static readonly Emoji gear = new("\u2699" + emojiStyle.Value, "Gear");
        public static readonly Emoji atomSymbol = new("\u269B" + emojiStyle.Value, "Atom Symbol");
        public static readonly Emoji keyboard = new("\u2328" + emojiStyle.Value, "Keyboard");
        public static readonly Emoji telephone = new("\u260E" + emojiStyle.Value, "Telephone");
        public static readonly Emoji studioMicrophone = new(char.ConvertFromUtf32(0x1F399) + emojiStyle.Value, "Studio Microphone");
        public static readonly Emoji levelSlider = new(char.ConvertFromUtf32(0x1F39A) + emojiStyle.Value, "Level Slider");
        public static readonly Emoji controlKnobs = new(char.ConvertFromUtf32(0x1F39B) + emojiStyle.Value, "Control Knobs");
        public static readonly Emoji movieCamera = new(char.ConvertFromUtf32(0x1F3A5), "Movie Camera");
        public static readonly Emoji headphone = new(char.ConvertFromUtf32(0x1F3A7), "Headphone");
        public static readonly Emoji videoGame = new(char.ConvertFromUtf32(0x1F3AE), "Video Game");
        public static readonly Emoji lightBulb = new(char.ConvertFromUtf32(0x1F4A1), "Light Bulb");
        public static readonly Emoji computerDisk = new(char.ConvertFromUtf32(0x1F4BD), "Computer Disk");
        public static readonly Emoji floppyDisk = new(char.ConvertFromUtf32(0x1F4BE), "Floppy Disk");
        public static readonly Emoji opticalDisk = new(char.ConvertFromUtf32(0x1F4BF), "Optical Disk");
        public static readonly Emoji dvd = new(char.ConvertFromUtf32(0x1F4C0), "DVD");
        public static readonly Emoji telephoneReceiver = new(char.ConvertFromUtf32(0x1F4DE), "Telephone Receiver");
        public static readonly Emoji pager = new(char.ConvertFromUtf32(0x1F4DF), "Pager");
        public static readonly Emoji faxMachine = new(char.ConvertFromUtf32(0x1F4E0), "Fax Machine");
        public static readonly Emoji satelliteAntenna = new(char.ConvertFromUtf32(0x1F4E1), "Satellite Antenna");
        public static readonly Emoji loudspeaker = new(char.ConvertFromUtf32(0x1F4E2), "Loudspeaker");
        public static readonly Emoji megaphone = new(char.ConvertFromUtf32(0x1F4E3), "Megaphone");
        public static readonly Emoji mobilePhone = new(char.ConvertFromUtf32(0x1F4F1), "Mobile Phone");
        public static readonly Emoji mobilePhoneWithArrow = new(char.ConvertFromUtf32(0x1F4F2), "Mobile Phone with Arrow");
        public static readonly Emoji mobilePhoneVibrating = new(char.ConvertFromUtf32(0x1F4F3), "Mobile Phone Vibrating");
        public static readonly Emoji mobilePhoneOff = new(char.ConvertFromUtf32(0x1F4F4), "Mobile Phone Off");
        public static readonly Emoji noMobilePhone = new(char.ConvertFromUtf32(0x1F4F5), "No Mobile Phone");
        public static readonly Emoji antennaBars = new(char.ConvertFromUtf32(0x1F4F6), "Antenna Bars");
        public static readonly Emoji camera = new(char.ConvertFromUtf32(0x1F4F7), "Camera");
        public static readonly Emoji cameraWithFlash = new(char.ConvertFromUtf32(0x1F4F8), "Camera with Flash");
        public static readonly Emoji videoCamera = new(char.ConvertFromUtf32(0x1F4F9), "Video Camera");
        public static readonly Emoji television = new(char.ConvertFromUtf32(0x1F4FA), "Television");
        public static readonly Emoji radio = new(char.ConvertFromUtf32(0x1F4FB), "Radio");
        public static readonly Emoji videocassette = new(char.ConvertFromUtf32(0x1F4FC), "Videocassette");
        public static readonly Emoji filmProjector = new(char.ConvertFromUtf32(0x1F4FD) + emojiStyle.Value, "Film Projector");
        public static readonly Emoji portableStereo = new(char.ConvertFromUtf32(0x1F4FE) + emojiStyle.Value, "Portable Stereo");
        public static readonly Emoji dimButton = new(char.ConvertFromUtf32(0x1F505), "Dim Button");
        public static readonly Emoji brightButton = new(char.ConvertFromUtf32(0x1F506), "Bright Button");
        public static readonly Emoji mutedSpeaker = new(char.ConvertFromUtf32(0x1F507), "Muted Speaker");
        public static readonly Emoji speakerLowVolume = new(char.ConvertFromUtf32(0x1F508), "Speaker Low Volume");
        public static readonly Emoji speakerMediumVolume = new(char.ConvertFromUtf32(0x1F509), "Speaker Medium Volume");
        public static readonly Emoji speakerHighVolume = new(char.ConvertFromUtf32(0x1F50A), "Speaker High Volume");
        public static readonly Emoji battery = new(char.ConvertFromUtf32(0x1F50B), "Battery");
        public static readonly Emoji electricPlug = new(char.ConvertFromUtf32(0x1F50C), "Electric Plug");
        public static readonly Emoji magnifyingGlassTiltedLeft = new(char.ConvertFromUtf32(0x1F50D), "Magnifying Glass Tilted Left");
        public static readonly Emoji magnifyingGlassTiltedRight = new(char.ConvertFromUtf32(0x1F50E), "Magnifying Glass Tilted Right");
        public static readonly Emoji lockedWithPen = new(char.ConvertFromUtf32(0x1F50F), "Locked with Pen");
        public static readonly Emoji lockedWithKey = new(char.ConvertFromUtf32(0x1F510), "Locked with Key");
        public static readonly Emoji key = new(char.ConvertFromUtf32(0x1F511), "Key");
        public static readonly Emoji locked = new(char.ConvertFromUtf32(0x1F512), "Locked");
        public static readonly Emoji unlocked = new(char.ConvertFromUtf32(0x1F513), "Unlocked");
        public static readonly Emoji bell = new(char.ConvertFromUtf32(0x1F514), "Bell");
        public static readonly Emoji bellWithSlash = new(char.ConvertFromUtf32(0x1F515), "Bell with Slash");
        public static readonly Emoji bookmark = new(char.ConvertFromUtf32(0x1F516), "Bookmark");
        public static readonly Emoji link = new(char.ConvertFromUtf32(0x1F517), "Link");
        public static readonly Emoji joystick = new(char.ConvertFromUtf32(0x1F579) + emojiStyle.Value, "Joystick");
        public static readonly Emoji desktopComputer = new(char.ConvertFromUtf32(0x1F5A5) + emojiStyle.Value, "Desktop Computer");
        public static readonly Emoji printer = new(char.ConvertFromUtf32(0x1F5A8) + emojiStyle.Value, "Printer");
        public static readonly Emoji computerMouse = new(char.ConvertFromUtf32(0x1F5B1) + emojiStyle.Value, "Computer Mouse");
        public static readonly Emoji trackball = new(char.ConvertFromUtf32(0x1F5B2) + emojiStyle.Value, "Trackball");
        public static readonly Emoji blackFolder = new(char.ConvertFromUtf32(0x1F5BF), "Black Folder");
        public static readonly Emoji folder = new(char.ConvertFromUtf32(0x1F5C0), "Folder");
        public static readonly Emoji openFolder = new(char.ConvertFromUtf32(0x1F5C1), "Open Folder");
        public static readonly Emoji cardIndexDividers = new(char.ConvertFromUtf32(0x1F5C2), "Card Index Dividers");
        public static readonly Emoji cardFileBox = new(char.ConvertFromUtf32(0x1F5C3), "Card File Box");
        public static readonly Emoji fileCabinet = new(char.ConvertFromUtf32(0x1F5C4), "File Cabinet");
        public static readonly Emoji emptyNote = new(char.ConvertFromUtf32(0x1F5C5), "Empty Note");
        public static readonly Emoji emptyNotePage = new(char.ConvertFromUtf32(0x1F5C6), "Empty Note Page");
        public static readonly Emoji emptyNotePad = new(char.ConvertFromUtf32(0x1F5C7), "Empty Note Pad");
        public static readonly Emoji note = new(char.ConvertFromUtf32(0x1F5C8), "Note");
        public static readonly Emoji notePage = new(char.ConvertFromUtf32(0x1F5C9), "Note Page");
        public static readonly Emoji notePad = new(char.ConvertFromUtf32(0x1F5CA), "Note Pad");
        public static readonly Emoji emptyDocument = new(char.ConvertFromUtf32(0x1F5CB), "Empty Document");
        public static readonly Emoji emptyPage = new(char.ConvertFromUtf32(0x1F5CC), "Empty Page");
        public static readonly Emoji emptyPages = new(char.ConvertFromUtf32(0x1F5CD), "Empty Pages");
        public static readonly Emoji documentIcon = new(char.ConvertFromUtf32(0x1F5CE), "Document");
        public static readonly Emoji page = new(char.ConvertFromUtf32(0x1F5CF), "Page");
        public static readonly Emoji pages = new(char.ConvertFromUtf32(0x1F5D0), "Pages");
        public static readonly Emoji wastebasket = new(char.ConvertFromUtf32(0x1F5D1), "Wastebasket");
        public static readonly Emoji spiralNotePad = new(char.ConvertFromUtf32(0x1F5D2), "Spiral Note Pad");
        public static readonly Emoji spiralCalendar = new(char.ConvertFromUtf32(0x1F5D3), "Spiral Calendar");
        public static readonly Emoji desktopWindow = new(char.ConvertFromUtf32(0x1F5D4), "Desktop Window");
        public static readonly Emoji minimize = new(char.ConvertFromUtf32(0x1F5D5), "Minimize");
        public static readonly Emoji maximize = new(char.ConvertFromUtf32(0x1F5D6), "Maximize");
        public static readonly Emoji overlap = new(char.ConvertFromUtf32(0x1F5D7), "Overlap");
        public static readonly Emoji reload = new(char.ConvertFromUtf32(0x1F5D8), "Reload");
        public static readonly Emoji close = new(char.ConvertFromUtf32(0x1F5D9), "Close");
        public static readonly Emoji increaseFontSize = new(char.ConvertFromUtf32(0x1F5DA), "Increase Font Size");
        public static readonly Emoji decreaseFontSize = new(char.ConvertFromUtf32(0x1F5DB), "Decrease Font Size");
        public static readonly Emoji compression = new(char.ConvertFromUtf32(0x1F5DC), "Compression");
        public static readonly Emoji oldKey = new(char.ConvertFromUtf32(0x1F5DD), "Old Key");
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
            new(char.ConvertFromUtf32(0x1F4E4), "Outbox Tray"),
            new(char.ConvertFromUtf32(0x1F4E5), "Inbox Tray"),
            new(char.ConvertFromUtf32(0x1F4E6), "Package"),
            new(char.ConvertFromUtf32(0x1F4E7), "E-Mail"),
            new(char.ConvertFromUtf32(0x1F4E8), "Incoming Envelope"),
            new(char.ConvertFromUtf32(0x1F4E9), "Envelope with Arrow"),
            new(char.ConvertFromUtf32(0x1F4EA), "Closed Mailbox with Lowered Flag"),
            new(char.ConvertFromUtf32(0x1F4EB), "Closed Mailbox with Raised Flag"),
            new(char.ConvertFromUtf32(0x1F4EC), "Open Mailbox with Raised Flag"),
            new(char.ConvertFromUtf32(0x1F4ED), "Open Mailbox with Lowered Flag"),
            new(char.ConvertFromUtf32(0x1F4EE), "Postbox"),
            new(char.ConvertFromUtf32(0x1F4EF), "Postal Horn"));

        public static readonly EmojiGroup celebration = new(
            "Celebration", "Celebration",
            new(char.ConvertFromUtf32(0x1F380), "Ribbon"),
            new(char.ConvertFromUtf32(0x1F381), "Wrapped Gift"),
            new(char.ConvertFromUtf32(0x1F383), "Jack-O-Lantern"),
            new(char.ConvertFromUtf32(0x1F384), "Christmas Tree"),
            new(char.ConvertFromUtf32(0x1F9E8), "Firecracker"),
            new(char.ConvertFromUtf32(0x1F386), "Fireworks"),
            new(char.ConvertFromUtf32(0x1F387), "Sparkler"),
            new("\u2728", "Sparkles"),
            new("\u2747" + emojiStyle.Value, "Sparkle"),
            new(char.ConvertFromUtf32(0x1F388), "Balloon"),
            new(char.ConvertFromUtf32(0x1F389), "Party Popper"),
            new(char.ConvertFromUtf32(0x1F38A), "Confetti Ball"),
            new(char.ConvertFromUtf32(0x1F38B), "Tanabata Tree"),
            new(char.ConvertFromUtf32(0x1F38D), "Pine Decoration"),
            new(char.ConvertFromUtf32(0x1F38E), "Japanese Dolls"),
            new(char.ConvertFromUtf32(0x1F38F), "Carp Streamer"),
            new(char.ConvertFromUtf32(0x1F390), "Wind Chime"),
            new(char.ConvertFromUtf32(0x1F391), "Moon Viewing Ceremony"),
            new(char.ConvertFromUtf32(0x1F392), "Backpack"),
            graduationCap,
            new(char.ConvertFromUtf32(0x1F9E7), "Red Envelope"),
            new(char.ConvertFromUtf32(0x1F3EE), "Red Paper Lantern"),
            new(char.ConvertFromUtf32(0x1F396) + emojiStyle.Value, "Military Medal"));

        public static readonly EmojiGroup tools = new(
            "Tools", "Tools",
            new(char.ConvertFromUtf32(0x1F3A3), "Fishing Pole"),
            new(char.ConvertFromUtf32(0x1F526), "Flashlight"),
            wrench,
            new(char.ConvertFromUtf32(0x1F528), "Hammer"),
            new(char.ConvertFromUtf32(0x1F529), "Nut and Bolt"),
            new(char.ConvertFromUtf32(0x1F6E0) + emojiStyle.Value, "Hammer and Wrench"),
            new(char.ConvertFromUtf32(0x1F9ED), "Compass"),
            new(char.ConvertFromUtf32(0x1F9EF), "Fire Extinguisher"),
            new(char.ConvertFromUtf32(0x1F9F0), "Toolbox"),
            new(char.ConvertFromUtf32(0x1F9F1), "Brick"),
            new(char.ConvertFromUtf32(0x1FA93), "Axe"),
            new("\u2692" + emojiStyle.Value, "Hammer and Pick"),
            new("\u26CF" + emojiStyle.Value, "Pick"),
            new("\u26D1" + emojiStyle.Value, "Rescue Worker’s Helmet"),
            new("\u26D3" + emojiStyle.Value, "Chains"),
            compression);

        public static readonly EmojiGroup office = new(
            "Office", "Office",
            new(char.ConvertFromUtf32(0x1F4C1), "File Folder"),
            new(char.ConvertFromUtf32(0x1F4C2), "Open File Folder"),
            new(char.ConvertFromUtf32(0x1F4C3), "Page with Curl"),
            new(char.ConvertFromUtf32(0x1F4C4), "Page Facing Up"),
            new(char.ConvertFromUtf32(0x1F4C5), "Calendar"),
            new(char.ConvertFromUtf32(0x1F4C6), "Tear-Off Calendar"),
            new(char.ConvertFromUtf32(0x1F4C7), "Card Index"),
            cardIndexDividers,
            cardFileBox,
            fileCabinet,
            wastebasket,
            spiralNotePad,
            spiralCalendar,
            new(char.ConvertFromUtf32(0x1F4C8), "Chart Increasing"),
            new(char.ConvertFromUtf32(0x1F4C9), "Chart Decreasing"),
            new(char.ConvertFromUtf32(0x1F4CA), "Bar Chart"),
            new(char.ConvertFromUtf32(0x1F4CB), "Clipboard"),
            new(char.ConvertFromUtf32(0x1F4CC), "Pushpin"),
            new(char.ConvertFromUtf32(0x1F4CD), "Round Pushpin"),
            new(char.ConvertFromUtf32(0x1F4CE), "Paperclip"),
            new(char.ConvertFromUtf32(0x1F587) + emojiStyle.Value, "Linked Paperclips"),
            new(char.ConvertFromUtf32(0x1F4CF), "Straight Ruler"),
            new(char.ConvertFromUtf32(0x1F4D0), "Triangular Ruler"),
            new(char.ConvertFromUtf32(0x1F4D1), "Bookmark Tabs"),
            new(char.ConvertFromUtf32(0x1F4D2), "Ledger"),
            new(char.ConvertFromUtf32(0x1F4D3), "Notebook"),
            new(char.ConvertFromUtf32(0x1F4D4), "Notebook with Decorative Cover"),
            new(char.ConvertFromUtf32(0x1F4D5), "Closed Book"),
            new(char.ConvertFromUtf32(0x1F4D6), "Open Book"),
            new(char.ConvertFromUtf32(0x1F4D7), "Green Book"),
            new(char.ConvertFromUtf32(0x1F4D8), "Blue Book"),
            new(char.ConvertFromUtf32(0x1F4D9), "Orange Book"),
            new(char.ConvertFromUtf32(0x1F4DA), "Books"),
            new(char.ConvertFromUtf32(0x1F4DB), "Name Badge"),
            new(char.ConvertFromUtf32(0x1F4DC), "Scroll"),
            new(char.ConvertFromUtf32(0x1F4DD), "Memo"),
            new("\u2702" + emojiStyle.Value, "Scissors"),
            new("\u2709" + emojiStyle.Value, "Envelope"));

        public static readonly EmojiGroup signs = new(
            "Signs", "Signs",
            new(char.ConvertFromUtf32(0x1F3A6), "Cinema"),
            noMobilePhone,
            new(char.ConvertFromUtf32(0x1F51E), "No One Under Eighteen"),
            new(char.ConvertFromUtf32(0x1F6AB), "Prohibited"),
            new(char.ConvertFromUtf32(0x1F6AC), "Cigarette"),
            new(char.ConvertFromUtf32(0x1F6AD), "No Smoking"),
            new(char.ConvertFromUtf32(0x1F6AE), "Litter in Bin Sign"),
            new(char.ConvertFromUtf32(0x1F6AF), "No Littering"),
            new(char.ConvertFromUtf32(0x1F6B0), "Potable Water"),
            new(char.ConvertFromUtf32(0x1F6B1), "Non-Potable Water"),
            new(char.ConvertFromUtf32(0x1F6B3), "No Bicycles"),
            new(char.ConvertFromUtf32(0x1F6B7), "No Pedestrians"),
            new(char.ConvertFromUtf32(0x1F6B8), "Children Crossing"),
            new(char.ConvertFromUtf32(0x1F6B9), "Men’s Room"),
            new(char.ConvertFromUtf32(0x1F6BA), "Women’s Room"),
            new(char.ConvertFromUtf32(0x1F6BB), "Restroom"),
            new(char.ConvertFromUtf32(0x1F6BC), "Baby Symbol"),
            new(char.ConvertFromUtf32(0x1F6BE), "Water Closet"),
            new(char.ConvertFromUtf32(0x1F6C2), "Passport Control"),
            new(char.ConvertFromUtf32(0x1F6C3), "Customs"),
            new(char.ConvertFromUtf32(0x1F6C4), "Baggage Claim"),
            new(char.ConvertFromUtf32(0x1F6C5), "Left Luggage"),
            new(char.ConvertFromUtf32(0x1F17F) + emojiStyle.Value, "Parking Button"),
            new("\u267F", "Wheelchair Symbol"),
            new("\u2622" + emojiStyle.Value, "Radioactive"),
            new("\u2623" + emojiStyle.Value, "Biohazard"),
            new("\u26A0" + emojiStyle.Value, "Warning"),
            new("\u26A1", "High Voltage"),
            new("\u26D4", "No Entry"),
            new("\u267B" + emojiStyle.Value, "Recycling Symbol"),
            female,
            male,
            transgender);

        public static readonly EmojiGroup religion = new(
            "Religion", "Religion",
            new(char.ConvertFromUtf32(0x1F52F), "Dotted Six-Pointed Star"),
            new("\u2721" + emojiStyle.Value, "Star of David"),
            new(char.ConvertFromUtf32(0x1F549) + emojiStyle.Value, "Om"),
            new(char.ConvertFromUtf32(0x1F54B), "Kaaba"),
            new(char.ConvertFromUtf32(0x1F54C), "Mosque"),
            new(char.ConvertFromUtf32(0x1F54D), "Synagogue"),
            new(char.ConvertFromUtf32(0x1F54E), "Menorah"),
            new(char.ConvertFromUtf32(0x1F6D0), "Place of Worship"),
            new(char.ConvertFromUtf32(0x1F6D5), "Hindu Temple"),
            new("\u2626" + emojiStyle.Value, "Orthodox Cross"),
            new("\u271D" + emojiStyle.Value, "Latin Cross"),
            new("\u262A" + emojiStyle.Value, "Star and Crescent"),
            new("\u262E" + emojiStyle.Value, "Peace Symbol"),
            new("\u262F" + emojiStyle.Value, "Yin Yang"),
            new("\u2638" + emojiStyle.Value, "Wheel of Dharma"),
            new("\u267E" + emojiStyle.Value, "Infinity"),
            new(char.ConvertFromUtf32(0x1FA94), "Diya Lamp"),
            new("\u26E9" + emojiStyle.Value, "Shinto Shrine"),
            new("\u26EA", "Church"),
            new("\u2734" + emojiStyle.Value, "Eight-Pointed Star"),
            new(char.ConvertFromUtf32(0x1F4FF), "Prayer Beads"));

        public static readonly Emoji door = new(char.ConvertFromUtf32(0x1F6AA), "Door");
        public static readonly EmojiGroup household = new(
            "Household", "Household",
            new(char.ConvertFromUtf32(0x1F484), "Lipstick"),
            new(char.ConvertFromUtf32(0x1F48D), "Ring"),
            new(char.ConvertFromUtf32(0x1F48E), "Gem Stone"),
            new(char.ConvertFromUtf32(0x1F4F0), "Newspaper"),
            key,
            new(char.ConvertFromUtf32(0x1F525), "Fire"),
            new(char.ConvertFromUtf32(0x1F52B), "Pistol"),
            new(char.ConvertFromUtf32(0x1F56F) + emojiStyle.Value, "Candle"),
            new(char.ConvertFromUtf32(0x1F5BC) + emojiStyle.Value, "Framed Picture"),
            oldKey,
            new(char.ConvertFromUtf32(0x1F5DE) + emojiStyle.Value, "Rolled-Up Newspaper"),
            new(char.ConvertFromUtf32(0x1F5FA) + emojiStyle.Value, "World Map"),
            door,
            new(char.ConvertFromUtf32(0x1F6BD), "Toilet"),
            new(char.ConvertFromUtf32(0x1F6BF), "Shower"),
            new(char.ConvertFromUtf32(0x1F6C1), "Bathtub"),
            new(char.ConvertFromUtf32(0x1F6CB) + emojiStyle.Value, "Couch and Lamp"),
            new(char.ConvertFromUtf32(0x1F6CF) + emojiStyle.Value, "Bed"),
            new(char.ConvertFromUtf32(0x1F9F4), "Lotion Bottle"),
            new(char.ConvertFromUtf32(0x1F9F5), "Thread"),
            new(char.ConvertFromUtf32(0x1F9F6), "Yarn"),
            new(char.ConvertFromUtf32(0x1F9F7), "Safety Pin"),
            new(char.ConvertFromUtf32(0x1F9F8), "Teddy Bear"),
            new(char.ConvertFromUtf32(0x1F9F9), "Broom"),
            new(char.ConvertFromUtf32(0x1F9FA), "Basket"),
            new(char.ConvertFromUtf32(0x1F9FB), "Roll of Paper"),
            new(char.ConvertFromUtf32(0x1F9FC), "Soap"),
            new(char.ConvertFromUtf32(0x1F9FD), "Sponge"),
            new(char.ConvertFromUtf32(0x1FA91), "Chair"),
            new(char.ConvertFromUtf32(0x1FA92), "Razor"),
            new(char.ConvertFromUtf32(0x1F397) + emojiStyle.Value, "Reminder Ribbon"));

        public static readonly EmojiGroup activities = new(
            "Activities", "Activities",
            new(char.ConvertFromUtf32(0x1F39E) + emojiStyle.Value, "Film Frames"),
            new(char.ConvertFromUtf32(0x1F39F) + emojiStyle.Value, "Admission Tickets"),
            new(char.ConvertFromUtf32(0x1F3A0), "Carousel Horse"),
            new(char.ConvertFromUtf32(0x1F3A1), "Ferris Wheel"),
            new(char.ConvertFromUtf32(0x1F3A2), "Roller Coaster"),
            artistPalette,
            new(char.ConvertFromUtf32(0x1F3AA), "Circus Tent"),
            new(char.ConvertFromUtf32(0x1F3AB), "Ticket"),
            new(char.ConvertFromUtf32(0x1F3AC), "Clapper Board"),
            new(char.ConvertFromUtf32(0x1F3AD), "Performing Arts"));

        public static readonly EmojiGroup travel = new(
            "Travel", "Travel",
            new(char.ConvertFromUtf32(0x1F3F7) + emojiStyle.Value, "Label"),
            new(char.ConvertFromUtf32(0x1F30B), "Volcano"),
            new(char.ConvertFromUtf32(0x1F3D4) + emojiStyle.Value, "Snow-Capped Mountain"),
            new("\u26F0" + emojiStyle.Value, "Mountain"),
            new(char.ConvertFromUtf32(0x1F3D5) + emojiStyle.Value, "Camping"),
            new(char.ConvertFromUtf32(0x1F3D6) + emojiStyle.Value, "Beach with Umbrella"),
            new("\u26F1" + emojiStyle.Value, "Umbrella on Ground"),
            new(char.ConvertFromUtf32(0x1F3EF), "Japanese Castle"),
            new(char.ConvertFromUtf32(0x1F463), "Footprints"),
            new(char.ConvertFromUtf32(0x1F5FB), "Mount Fuji"),
            new(char.ConvertFromUtf32(0x1F5FC), "Tokyo Tower"),
            new(char.ConvertFromUtf32(0x1F5FD), "Statue of Liberty"),
            new(char.ConvertFromUtf32(0x1F5FE), "Map of Japan"),
            new(char.ConvertFromUtf32(0x1F5FF), "Moai"),
            new(char.ConvertFromUtf32(0x1F6CE) + emojiStyle.Value, "Bellhop Bell"),
            new(char.ConvertFromUtf32(0x1F9F3), "Luggage"),
            new("\u26F3", "Flag in Hole"),
            new("\u26FA", "Tent"),
            new("\u2668" + emojiStyle.Value, "Hot Springs"));

        public static readonly EmojiGroup medieval = new(
            "Medieval", "Medieval",
            new(char.ConvertFromUtf32(0x1F3F0), "Castle"),
            new(char.ConvertFromUtf32(0x1F3F9), "Bow and Arrow"),
            crown,
            new(char.ConvertFromUtf32(0x1F531), "Trident Emblem"),
            new(char.ConvertFromUtf32(0x1F5E1) + emojiStyle.Value, "Dagger"),
            new(char.ConvertFromUtf32(0x1F6E1) + emojiStyle.Value, "Shield"),
            new(char.ConvertFromUtf32(0x1F52E), "Crystal Ball"),
            new("\u2694" + emojiStyle.Value, "Crossed Swords"),
            new("\u269C" + emojiStyle.Value, "Fleur-de-lis"));

        public static readonly Emoji doubleExclamationMark = new("\u203C" + emojiStyle.Value, "Double Exclamation Mark");
        public static readonly Emoji interrobang = new("\u2049" + emojiStyle.Value, "Exclamation Question Mark");
        public static readonly Emoji information = new("\u2139" + emojiStyle.Value, "Information");
        public static readonly Emoji circledM = new("\u24C2" + emojiStyle.Value, "Circled M");
        public static readonly Emoji checkMarkButton = new("\u2705", "Check Mark Button");
        public static readonly Emoji checkMark = new("\u2714" + emojiStyle.Value, "Check Mark");
        public static readonly Emoji eightSpokedAsterisk = new("\u2733" + emojiStyle.Value, "Eight-Spoked Asterisk");
        public static readonly Emoji crossMark = new("\u274C", "Cross Mark");
        public static readonly Emoji crossMarkButton = new("\u274E", "Cross Mark Button");
        public static readonly Emoji questionMark = new("\u2753", "Question Mark");
        public static readonly Emoji whiteQuestionMark = new("\u2754", "White Question Mark");
        public static readonly Emoji whiteExclamationMark = new("\u2755", "White Exclamation Mark");
        public static readonly Emoji exclamationMark = new("\u2757", "Exclamation Mark");
        public static readonly Emoji curlyLoop = new("\u27B0", "Curly Loop");
        public static readonly Emoji doubleCurlyLoop = new("\u27BF", "Double Curly Loop");
        public static readonly Emoji wavyDash = new("\u3030" + emojiStyle.Value, "Wavy Dash");
        public static readonly Emoji partAlternationMark = new("\u303D" + emojiStyle.Value, "Part Alternation Mark");
        public static readonly Emoji tradeMark = new("\u2122" + emojiStyle.Value, "Trade Mark");
        public static readonly Emoji copyright = new("\u00A9" + emojiStyle.Value, "Copyright");
        public static readonly Emoji registered = new("\u00AE" + emojiStyle.Value, "Registered");
        public static readonly Emoji squareFourCourners = new("\u26F6" + emojiStyle.Value, "Square: Four Corners");

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

        public static readonly Emoji droplet = new(char.ConvertFromUtf32(0x1F4A7), "Droplet");
        public static readonly Emoji dropOfBlood = new(char.ConvertFromUtf32(0x1FA78), "Drop of Blood");
        public static readonly Emoji adhesiveBandage = new(char.ConvertFromUtf32(0x1FA79), "Adhesive Bandage");
        public static readonly Emoji stethoscope = new(char.ConvertFromUtf32(0x1FA7A), "Stethoscope");
        public static readonly Emoji syringe = new(char.ConvertFromUtf32(0x1F489), "Syringe");
        public static readonly Emoji pill = new(char.ConvertFromUtf32(0x1F48A), "Pill");
        public static readonly Emoji testTube = new(char.ConvertFromUtf32(0x1F9EA), "Test Tube");
        public static readonly Emoji petriDish = new(char.ConvertFromUtf32(0x1F9EB), "Petri Dish");
        public static readonly Emoji dna = new(char.ConvertFromUtf32(0x1F9EC), "DNA");
        public static readonly Emoji abacus = new(char.ConvertFromUtf32(0x1F9EE), "Abacus");
        public static readonly Emoji magnet = new(char.ConvertFromUtf32(0x1F9F2), "Magnet");
        public static readonly Emoji telescope = new(char.ConvertFromUtf32(0x1F52D), "Telescope");

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
            whiteChessKing.Value + whiteChessQueen.Value + whiteChessRook.Value + whiteChessBishop.Value + whiteChessKnight.Value + whiteChessPawn.Value,
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
            blackChessKing.Value + blackChessQueen.Value + blackChessRook.Value + blackChessBishop.Value + blackChessKnight.Value + blackChessPawn.Value,
            "Black Chess Pieces",
            blackChessKing,
            blackChessQueen,
            blackChessRook,
            blackChessBishop,
            blackChessKnight,
            blackChessPawn);

        public static readonly EmojiGroup chessPawns = new(
            whiteChessPawn.Value + blackChessPawn.Value,
            "Chess Pawns",
            whiteChessPawn,
            blackChessPawn);

        public static readonly EmojiGroup chessRooks = new(
            whiteChessRook.Value + blackChessRook.Value,
            "Chess Rooks",
            whiteChessRook,
            blackChessRook);

        public static readonly EmojiGroup chessBishops = new(
            whiteChessBishop.Value + blackChessBishop.Value,
            "Chess Bishops",
            whiteChessBishop,
            blackChessBishop);

        public static readonly EmojiGroup chessKnights = new(
            whiteChessKnight.Value + blackChessKnight.Value,
            "Chess Knights",
            whiteChessKnight,
            blackChessKnight);

        public static readonly EmojiGroup chessQueens = new(
            whiteChessQueen.Value + blackChessQueen.Value,
            "Chess Queens",
            whiteChessQueen,
            blackChessQueen);

        public static readonly EmojiGroup chessKings = new(
            whiteChessKing.Value + blackChessKing.Value,
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

        public static readonly Emoji crossedFlags = new(char.ConvertFromUtf32(0x1F38C), "Crossed Flags");
        public static readonly Emoji chequeredFlag = new(char.ConvertFromUtf32(0x1F3C1), "Chequered Flag");
        public static readonly Emoji whiteFlag = new(char.ConvertFromUtf32(0x1F3F3) + emojiStyle.Value, "White Flag");
        public static readonly Emoji blackFlag = new(char.ConvertFromUtf32(0x1F3F4), "Black Flag");
        public static readonly Emoji rainbowFlag = Join(whiteFlag, rainbow, "Rainbow Flag");
        public static readonly Emoji transgenderFlag = Join(whiteFlag, transgender, "Transgender Flag");
        public static readonly Emoji pirateFlag = Join(blackFlag, skullAndCrossbones, "Pirate Flag");
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
        public static readonly Emoji triangularFlag = new(char.ConvertFromUtf32(0x1F6A9), "Triangular Flag");

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

        public static readonly Emoji cat = new(char.ConvertFromUtf32(0x1F408), "Cat");
        public static readonly Emoji dog = new(char.ConvertFromUtf32(0x1F415), "Dog");
        public static readonly Emoji bear = new(char.ConvertFromUtf32(0x1F43B), "Bear");
        public static readonly Emoji blackCat = Join(cat, blackLargeSquare, "Black Cat");
        public static readonly Emoji serviceDog = Join(dog, safetyVest, "Service Dog");
        public static readonly Emoji polarBear = Join(bear, snowflake, "Polar Bear");

        public static readonly EmojiGroup animals = new(
            "Animals", "Animals and insects",
            new(char.ConvertFromUtf32(0x1F400), "Rat"),
            new(char.ConvertFromUtf32(0x1F401), "Mouse"),
            new(char.ConvertFromUtf32(0x1F402), "Ox"),
            new(char.ConvertFromUtf32(0x1F403), "Water Buffalo"),
            new(char.ConvertFromUtf32(0x1F404), "Cow"),
            new(char.ConvertFromUtf32(0x1F405), "Tiger"),
            new(char.ConvertFromUtf32(0x1F406), "Leopard"),
            new(char.ConvertFromUtf32(0x1F407), "Rabbit"),
            cat,
            blackCat,
            new(char.ConvertFromUtf32(0x1F409), "Dragon"),
            new(char.ConvertFromUtf32(0x1F40A), "Crocodile"),
            new(char.ConvertFromUtf32(0x1F40B), "Whale"),
            new(char.ConvertFromUtf32(0x1F40C), "Snail"),
            new(char.ConvertFromUtf32(0x1F40D), "Snake"),
            new(char.ConvertFromUtf32(0x1F40E), "Horse"),
            new(char.ConvertFromUtf32(0x1F40F), "Ram"),
            new(char.ConvertFromUtf32(0x1F410), "Goat"),
            new(char.ConvertFromUtf32(0x1F411), "Ewe"),
            new(char.ConvertFromUtf32(0x1F412), "Monkey"),
            new(char.ConvertFromUtf32(0x1F413), "Rooster"),
            new(char.ConvertFromUtf32(0x1F414), "Chicken"),
            dog,
            serviceDog,
            new(char.ConvertFromUtf32(0x1F416), "Pig"),
            new(char.ConvertFromUtf32(0x1F417), "Boar"),
            new(char.ConvertFromUtf32(0x1F418), "Elephant"),
            new(char.ConvertFromUtf32(0x1F419), "Octopus"),
            new(char.ConvertFromUtf32(0x1F41A), "Spiral Shell"),
            new(char.ConvertFromUtf32(0x1F41B), "Bug"),
            new(char.ConvertFromUtf32(0x1F41C), "Ant"),
            new(char.ConvertFromUtf32(0x1F41D), "Honeybee"),
            new(char.ConvertFromUtf32(0x1F41E), "Lady Beetle"),
            new(char.ConvertFromUtf32(0x1F41F), "Fish"),
            new(char.ConvertFromUtf32(0x1F420), "Tropical Fish"),
            new(char.ConvertFromUtf32(0x1F421), "Blowfish"),
            new(char.ConvertFromUtf32(0x1F422), "Turtle"),
            new(char.ConvertFromUtf32(0x1F423), "Hatching Chick"),
            new(char.ConvertFromUtf32(0x1F424), "Baby Chick"),
            new(char.ConvertFromUtf32(0x1F425), "Front-Facing Baby Chick"),
            new(char.ConvertFromUtf32(0x1F426), "Bird"),
            new(char.ConvertFromUtf32(0x1F427), "Penguin"),
            new(char.ConvertFromUtf32(0x1F428), "Koala"),
            new(char.ConvertFromUtf32(0x1F429), "Poodle"),
            new(char.ConvertFromUtf32(0x1F42A), "Camel"),
            new(char.ConvertFromUtf32(0x1F42B), "Two-Hump Camel"),
            new(char.ConvertFromUtf32(0x1F42C), "Dolphin"),
            new(char.ConvertFromUtf32(0x1F42D), "Mouse Face"),
            new(char.ConvertFromUtf32(0x1F42E), "Cow Face"),
            new(char.ConvertFromUtf32(0x1F42F), "Tiger Face"),
            new(char.ConvertFromUtf32(0x1F430), "Rabbit Face"),
            new(char.ConvertFromUtf32(0x1F431), "Cat Face"),
            new(char.ConvertFromUtf32(0x1F432), "Dragon Face"),
            new(char.ConvertFromUtf32(0x1F433), "Spouting Whale"),
            new(char.ConvertFromUtf32(0x1F434), "Horse Face"),
            new(char.ConvertFromUtf32(0x1F435), "Monkey Face"),
            new(char.ConvertFromUtf32(0x1F436), "Dog Face"),
            new(char.ConvertFromUtf32(0x1F437), "Pig Face"),
            new(char.ConvertFromUtf32(0x1F438), "Frog"),
            new(char.ConvertFromUtf32(0x1F439), "Hamster"),
            new(char.ConvertFromUtf32(0x1F43A), "Wolf"),
            bear,
            polarBear,
            new(char.ConvertFromUtf32(0x1F43C), "Panda"),
            new(char.ConvertFromUtf32(0x1F43D), "Pig Nose"),
            new(char.ConvertFromUtf32(0x1F43E), "Paw Prints"),
            new(char.ConvertFromUtf32(0x1F43F) + emojiStyle.Value, "Chipmunk"),
            new(char.ConvertFromUtf32(0x1F54A) + emojiStyle.Value, "Dove"),
            new(char.ConvertFromUtf32(0x1F577) + emojiStyle.Value, "Spider"),
            new(char.ConvertFromUtf32(0x1F578) + emojiStyle.Value, "Spider Web"),
            new(char.ConvertFromUtf32(0x1F981), "Lion"),
            new(char.ConvertFromUtf32(0x1F982), "Scorpion"),
            new(char.ConvertFromUtf32(0x1F983), "Turkey"),
            new(char.ConvertFromUtf32(0x1F984), "Unicorn"),
            new(char.ConvertFromUtf32(0x1F985), "Eagle"),
            new(char.ConvertFromUtf32(0x1F986), "Duck"),
            new(char.ConvertFromUtf32(0x1F987), "Bat"),
            new(char.ConvertFromUtf32(0x1F988), "Shark"),
            new(char.ConvertFromUtf32(0x1F989), "Owl"),
            new(char.ConvertFromUtf32(0x1F98A), "Fox"),
            new(char.ConvertFromUtf32(0x1F98B), "Butterfly"),
            new(char.ConvertFromUtf32(0x1F98C), "Deer"),
            new(char.ConvertFromUtf32(0x1F98D), "Gorilla"),
            new(char.ConvertFromUtf32(0x1F98E), "Lizard"),
            new(char.ConvertFromUtf32(0x1F98F), "Rhinoceros"),
            new(char.ConvertFromUtf32(0x1F992), "Giraffe"),
            new(char.ConvertFromUtf32(0x1F993), "Zebra"),
            new(char.ConvertFromUtf32(0x1F994), "Hedgehog"),
            new(char.ConvertFromUtf32(0x1F995), "Sauropod"),
            new(char.ConvertFromUtf32(0x1F996), "T-Rex"),
            new(char.ConvertFromUtf32(0x1F997), "Cricket"),
            new(char.ConvertFromUtf32(0x1F998), "Kangaroo"),
            new(char.ConvertFromUtf32(0x1F999), "Llama"),
            new(char.ConvertFromUtf32(0x1F99A), "Peacock"),
            new(char.ConvertFromUtf32(0x1F99B), "Hippopotamus"),
            new(char.ConvertFromUtf32(0x1F99C), "Parrot"),
            new(char.ConvertFromUtf32(0x1F99D), "Raccoon"),
            new(char.ConvertFromUtf32(0x1F99F), "Mosquito"),
            new(char.ConvertFromUtf32(0x1F9A0), "Microbe"),
            new(char.ConvertFromUtf32(0x1F9A1), "Badger"),
            new(char.ConvertFromUtf32(0x1F9A2), "Swan"),
            new(char.ConvertFromUtf32(0x1F9A3), "Mammoth"),
            new(char.ConvertFromUtf32(0x1F9A4), "Dodo"),
            new(char.ConvertFromUtf32(0x1F9A5), "Sloth"),
            new(char.ConvertFromUtf32(0x1F9A6), "Otter"),
            new(char.ConvertFromUtf32(0x1F9A7), "Orangutan"),
            new(char.ConvertFromUtf32(0x1F9A8), "Skunk"),
            new(char.ConvertFromUtf32(0x1F9A9), "Flamingo"),
            new(char.ConvertFromUtf32(0x1F9AB), "Beaver"),
            new(char.ConvertFromUtf32(0x1F9AC), "Bison"),
            new(char.ConvertFromUtf32(0x1F9AD), "Seal"),
            new(char.ConvertFromUtf32(0x1FAB0), "Fly"),
            new(char.ConvertFromUtf32(0x1FAB1), "Worm"),
            new(char.ConvertFromUtf32(0x1FAB2), "Beetle"),
            new(char.ConvertFromUtf32(0x1FAB3), "Cockroach"),
            new(char.ConvertFromUtf32(0x1FAB6), "Feather"),
            new(char.ConvertFromUtf32(0x1F9AE), "Guide Dog"));

        public static readonly EmojiGroup nations = new(
            "National Flags", "Flags of countries from around the world",
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterC, "Flag: Ascension Island"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterD, "Flag: Andorra"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterE, "Flag: United Arab Emirates"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterF, "Flag: Afghanistan"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterG, "Flag: Antigua & Barbuda"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterI, "Flag: Anguilla"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterL, "Flag: Albania"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterM, "Flag: Armenia"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterO, "Flag: Angola"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterQ, "Flag: Antarctica"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterR, "Flag: Argentina"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterS, "Flag: American Samoa"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterT, "Flag: Austria"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterU, "Flag: Australia"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterW, "Flag: Aruba"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterX, "Flag: Åland Islands"),
            Combo(regionalIndicatorSymbolLetterA, regionalIndicatorSymbolLetterZ, "Flag: Azerbaijan"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterA, "Flag: Bosnia & Herzegovina"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterB, "Flag: Barbados"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterD, "Flag: Bangladesh"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterE, "Flag: Belgium"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterF, "Flag: Burkina Faso"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterG, "Flag: Bulgaria"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterH, "Flag: Bahrain"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterI, "Flag: Burundi"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterJ, "Flag: Benin"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterL, "Flag: St. Barthélemy"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterM, "Flag: Bermuda"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterN, "Flag: Brunei"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterO, "Flag: Bolivia"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterQ, "Flag: Caribbean Netherlands"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterR, "Flag: Brazil"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterS, "Flag: Bahamas"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterT, "Flag: Bhutan"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterV, "Flag: Bouvet Island"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterW, "Flag: Botswana"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterY, "Flag: Belarus"),
            Combo(regionalIndicatorSymbolLetterB, regionalIndicatorSymbolLetterZ, "Flag: Belize"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterA, "Flag: Canada"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterC, "Flag: Cocos (Keeling) Islands"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterD, "Flag: Congo - Kinshasa"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterF, "Flag: Central African Republic"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterG, "Flag: Congo - Brazzaville"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterH, "Flag: Switzerland"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterI, "Flag: Côte d’Ivoire"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterK, "Flag: Cook Islands"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterL, "Flag: Chile"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterM, "Flag: Cameroon"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterN, "Flag: China"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterO, "Flag: Colombia"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterP, "Flag: Clipperton Island"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterR, "Flag: Costa Rica"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterU, "Flag: Cuba"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterV, "Flag: Cape Verde"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterW, "Flag: Curaçao"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterX, "Flag: Christmas Island"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterY, "Flag: Cyprus"),
            Combo(regionalIndicatorSymbolLetterC, regionalIndicatorSymbolLetterZ, "Flag: Czechia"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterE, "Flag: Germany"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterG, "Flag: Diego Garcia"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterJ, "Flag: Djibouti"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterK, "Flag: Denmark"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterM, "Flag: Dominica"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterO, "Flag: Dominican Republic"),
            Combo(regionalIndicatorSymbolLetterD, regionalIndicatorSymbolLetterZ, "Flag: Algeria"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterA, "Flag: Ceuta & Melilla"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterC, "Flag: Ecuador"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterE, "Flag: Estonia"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterG, "Flag: Egypt"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterH, "Flag: Western Sahara"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterR, "Flag: Eritrea"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterS, "Flag: Spain"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterT, "Flag: Ethiopia"),
            Combo(regionalIndicatorSymbolLetterE, regionalIndicatorSymbolLetterU, "Flag: European Union"),
            Combo(regionalIndicatorSymbolLetterF, regionalIndicatorSymbolLetterI, "Flag: Finland"),
            Combo(regionalIndicatorSymbolLetterF, regionalIndicatorSymbolLetterJ, "Flag: Fiji"),
            Combo(regionalIndicatorSymbolLetterF, regionalIndicatorSymbolLetterK, "Flag: Falkland Islands"),
            Combo(regionalIndicatorSymbolLetterF, regionalIndicatorSymbolLetterM, "Flag: Micronesia"),
            Combo(regionalIndicatorSymbolLetterF, regionalIndicatorSymbolLetterO, "Flag: Faroe Islands"),
            Combo(regionalIndicatorSymbolLetterF, regionalIndicatorSymbolLetterR, "Flag: France"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterA, "Flag: Gabon"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterB, "Flag: United Kingdom"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterD, "Flag: Grenada"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterE, "Flag: Georgia"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterF, "Flag: French Guiana"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterG, "Flag: Guernsey"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterH, "Flag: Ghana"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterI, "Flag: Gibraltar"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterL, "Flag: Greenland"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterM, "Flag: Gambia"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterN, "Flag: Guinea"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterP, "Flag: Guadeloupe"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterQ, "Flag: Equatorial Guinea"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterR, "Flag: Greece"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterS, "Flag: South Georgia & South Sandwich Islands"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterT, "Flag: Guatemala"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterU, "Flag: Guam"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterW, "Flag: Guinea-Bissau"),
            Combo(regionalIndicatorSymbolLetterG, regionalIndicatorSymbolLetterY, "Flag: Guyana"),
            Combo(regionalIndicatorSymbolLetterH, regionalIndicatorSymbolLetterK, "Flag: Hong Kong SAR China"),
            Combo(regionalIndicatorSymbolLetterH, regionalIndicatorSymbolLetterM, "Flag: Heard & McDonald Islands"),
            Combo(regionalIndicatorSymbolLetterH, regionalIndicatorSymbolLetterN, "Flag: Honduras"),
            Combo(regionalIndicatorSymbolLetterH, regionalIndicatorSymbolLetterR, "Flag: Croatia"),
            Combo(regionalIndicatorSymbolLetterH, regionalIndicatorSymbolLetterT, "Flag: Haiti"),
            Combo(regionalIndicatorSymbolLetterH, regionalIndicatorSymbolLetterU, "Flag: Hungary"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterC, "Flag: Canary Islands"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterD, "Flag: Indonesia"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterE, "Flag: Ireland"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterL, "Flag: Israel"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterM, "Flag: Isle of Man"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterN, "Flag: India"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterO, "Flag: British Indian Ocean Territory"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterQ, "Flag: Iraq"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterR, "Flag: Iran"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterS, "Flag: Iceland"),
            Combo(regionalIndicatorSymbolLetterI, regionalIndicatorSymbolLetterT, "Flag: Italy"),
            Combo(regionalIndicatorSymbolLetterJ, regionalIndicatorSymbolLetterE, "Flag: Jersey"),
            Combo(regionalIndicatorSymbolLetterJ, regionalIndicatorSymbolLetterM, "Flag: Jamaica"),
            Combo(regionalIndicatorSymbolLetterJ, regionalIndicatorSymbolLetterO, "Flag: Jordan"),
            Combo(regionalIndicatorSymbolLetterJ, regionalIndicatorSymbolLetterP, "Flag: Japan"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterE, "Flag: Kenya"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterG, "Flag: Kyrgyzstan"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterH, "Flag: Cambodia"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterI, "Flag: Kiribati"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterM, "Flag: Comoros"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterN, "Flag: St. Kitts & Nevis"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterP, "Flag: North Korea"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterR, "Flag: South Korea"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterW, "Flag: Kuwait"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterY, "Flag: Cayman Islands"),
            Combo(regionalIndicatorSymbolLetterK, regionalIndicatorSymbolLetterZ, "Flag: Kazakhstan"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterA, "Flag: Laos"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterB, "Flag: Lebanon"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterC, "Flag: St. Lucia"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterI, "Flag: Liechtenstein"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterK, "Flag: Sri Lanka"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterR, "Flag: Liberia"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterS, "Flag: Lesotho"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterT, "Flag: Lithuania"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterU, "Flag: Luxembourg"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterV, "Flag: Latvia"),
            Combo(regionalIndicatorSymbolLetterL, regionalIndicatorSymbolLetterY, "Flag: Libya"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterA, "Flag: Morocco"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterC, "Flag: Monaco"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterD, "Flag: Moldova"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterE, "Flag: Montenegro"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterF, "Flag: St. Martin"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterG, "Flag: Madagascar"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterH, "Flag: Marshall Islands"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterK, "Flag: North Macedonia"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterL, "Flag: Mali"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterM, "Flag: Myanmar (Burma)"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterN, "Flag: Mongolia"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterO, "Flag: Macao Sar China"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterP, "Flag: Northern Mariana Islands"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterQ, "Flag: Martinique"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterR, "Flag: Mauritania"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterS, "Flag: Montserrat"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterT, "Flag: Malta"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterU, "Flag: Mauritius"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterV, "Flag: Maldives"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterW, "Flag: Malawi"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterX, "Flag: Mexico"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterY, "Flag: Malaysia"),
            Combo(regionalIndicatorSymbolLetterM, regionalIndicatorSymbolLetterZ, "Flag: Mozambique"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterA, "Flag: Namibia"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterC, "Flag: New Caledonia"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterE, "Flag: Niger"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterF, "Flag: Norfolk Island"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterG, "Flag: Nigeria"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterI, "Flag: Nicaragua"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterL, "Flag: Netherlands"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterO, "Flag: Norway"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterP, "Flag: Nepal"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterR, "Flag: Nauru"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterU, "Flag: Niue"),
            Combo(regionalIndicatorSymbolLetterN, regionalIndicatorSymbolLetterZ, "Flag: New Zealand"),
            Combo(regionalIndicatorSymbolLetterO, regionalIndicatorSymbolLetterM, "Flag: Oman"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterA, "Flag: Panama"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterE, "Flag: Peru"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterF, "Flag: French Polynesia"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterG, "Flag: Papua New Guinea"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterH, "Flag: Philippines"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterK, "Flag: Pakistan"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterL, "Flag: Poland"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterM, "Flag: St. Pierre & Miquelon"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterN, "Flag: Pitcairn Islands"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterR, "Flag: Puerto Rico"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterS, "Flag: Palestinian Territories"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterT, "Flag: Portugal"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterW, "Flag: Palau"),
            Combo(regionalIndicatorSymbolLetterP, regionalIndicatorSymbolLetterY, "Flag: Paraguay"),
            Combo(regionalIndicatorSymbolLetterQ, regionalIndicatorSymbolLetterA, "Flag: Qatar"),
            Combo(regionalIndicatorSymbolLetterR, regionalIndicatorSymbolLetterE, "Flag: Réunion"),
            Combo(regionalIndicatorSymbolLetterR, regionalIndicatorSymbolLetterO, "Flag: Romania"),
            Combo(regionalIndicatorSymbolLetterR, regionalIndicatorSymbolLetterS, "Flag: Serbia"),
            Combo(regionalIndicatorSymbolLetterR, regionalIndicatorSymbolLetterU, "Flag: Russia"),
            Combo(regionalIndicatorSymbolLetterR, regionalIndicatorSymbolLetterW, "Flag: Rwanda"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterA, "Flag: Saudi Arabia"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterB, "Flag: Solomon Islands"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterC, "Flag: Seychelles"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterD, "Flag: Sudan"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterE, "Flag: Sweden"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterG, "Flag: Singapore"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterH, "Flag: St. Helena"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterI, "Flag: Slovenia"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterJ, "Flag: Svalbard & Jan Mayen"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterK, "Flag: Slovakia"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterL, "Flag: Sierra Leone"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterM, "Flag: San Marino"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterN, "Flag: Senegal"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterO, "Flag: Somalia"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterR, "Flag: Suriname"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterS, "Flag: South Sudan"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterT, "Flag: São Tomé & Príncipe"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterV, "Flag: El Salvador"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterX, "Flag: Sint Maarten"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterY, "Flag: Syria"),
            Combo(regionalIndicatorSymbolLetterS, regionalIndicatorSymbolLetterZ, "Flag: Eswatini"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterA, "Flag: Tristan Da Cunha"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterC, "Flag: Turks & Caicos Islands"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterD, "Flag: Chad"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterF, "Flag: French Southern Territories"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterG, "Flag: Togo"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterH, "Flag: Thailand"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterJ, "Flag: Tajikistan"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterK, "Flag: Tokelau"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterL, "Flag: Timor-Leste"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterM, "Flag: Turkmenistan"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterN, "Flag: Tunisia"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterO, "Flag: Tonga"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterR, "Flag: Turkey"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterT, "Flag: Trinidad & Tobago"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterV, "Flag: Tuvalu"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterW, "Flag: Taiwan"),
            Combo(regionalIndicatorSymbolLetterT, regionalIndicatorSymbolLetterZ, "Flag: Tanzania"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterA, "Flag: Ukraine"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterG, "Flag: Uganda"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterM, "Flag: U.S. Outlying Islands"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterN, "Flag: United Nations"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterS, "Flag: United States"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterY, "Flag: Uruguay"),
            Combo(regionalIndicatorSymbolLetterU, regionalIndicatorSymbolLetterZ, "Flag: Uzbekistan"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterA, "Flag: Vatican City"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterC, "Flag: St. Vincent & Grenadines"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterE, "Flag: Venezuela"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterG, "Flag: British Virgin Islands"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterI, "Flag: U.S. Virgin Islands"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterN, "Flag: Vietnam"),
            Combo(regionalIndicatorSymbolLetterV, regionalIndicatorSymbolLetterU, "Flag: Vanuatu"),
            Combo(regionalIndicatorSymbolLetterW, regionalIndicatorSymbolLetterF, "Flag: Wallis & Futuna"),
            Combo(regionalIndicatorSymbolLetterW, regionalIndicatorSymbolLetterS, "Flag: Samoa"),
            Combo(regionalIndicatorSymbolLetterX, regionalIndicatorSymbolLetterK, "Flag: Kosovo"),
            Combo(regionalIndicatorSymbolLetterY, regionalIndicatorSymbolLetterE, "Flag: Yemen"),
            Combo(regionalIndicatorSymbolLetterY, regionalIndicatorSymbolLetterT, "Flag: Mayotte"),
            Combo(regionalIndicatorSymbolLetterZ, regionalIndicatorSymbolLetterA, "Flag: South Africa"),
            Combo(regionalIndicatorSymbolLetterZ, regionalIndicatorSymbolLetterM, "Flag: Zambia"),
            Combo(regionalIndicatorSymbolLetterZ, regionalIndicatorSymbolLetterW, "Flag: Zimbabwe"));

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
