using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class Emoji
    {
        public static readonly Emoji textStyle = new Emoji("\uFE0E", "Variation Selector-15: text style");
        public static readonly Emoji emojiStyle = new Emoji("\uFE0F", "Variation Selector-16: emoji style");
        public static readonly Emoji zeroWidthJoiner = new Emoji("\u200D", "Zero Width Joiner");
        public static readonly Emoji combiningEnclosingKeycap = new Emoji("\u20E3", "Combining Enclosing Keycap");
        public static readonly IReadOnlyList<Emoji> combiners = new[] {
            textStyle,
            emojiStyle,
            zeroWidthJoiner,
            combiningEnclosingKeycap,
        };

        public static readonly Emoji female = new Emoji("\u2640" + emojiStyle.Value, "Female");
        public static readonly Emoji male = new Emoji("\u2642" + emojiStyle.Value, "Male");
        public static readonly Emoji transgender = new Emoji("\u26A7" + emojiStyle.Value, "Transgender Symbol");
        public static readonly IReadOnlyList<Emoji> sexes = new[] {
            female,
            male,
        };
        public static readonly Emoji skinL = new Emoji(char.ConvertFromUtf32(0x1F3FB), "Light Skin Tone");
        public static readonly Emoji skinML = new Emoji(char.ConvertFromUtf32(0x1F3FC), "Medium-Light Skin Tone");
        public static readonly Emoji skinM = new Emoji(char.ConvertFromUtf32(0x1F3FD), "Medium Skin Tone");
        public static readonly Emoji skinMD = new Emoji(char.ConvertFromUtf32(0x1F3FE), "Medium-Dark Skin Tone");
        public static readonly Emoji skinD = new Emoji(char.ConvertFromUtf32(0x1F3FF), "Dark Skin Tone");
        public static readonly IReadOnlyList<Emoji> skinTones = new[] {
            skinL,
            skinML,
            skinM,
            skinMD,
            skinD,
        };
        public static readonly Emoji hairRed = new Emoji(char.ConvertFromUtf32(0x1F9B0), "Red Hair");
        public static readonly Emoji hairCurly = new Emoji(char.ConvertFromUtf32(0x1F9B1), "Curly Hair");
        public static readonly Emoji hairWhite = new Emoji(char.ConvertFromUtf32(0x1F9B3), "White Hair");
        public static readonly Emoji hairBald = new Emoji(char.ConvertFromUtf32(0x1F9B2), "Bald");
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
            return new Emoji(value, description);
        }

        private static Emoji Combo(Emoji a, Emoji b, string altDesc = null)
        {
            return new Emoji(a.Value + b.Value, altDesc ?? a.Desc + b.Desc);
        }

        private static Emoji Join(Emoji a, Emoji b, string altDesc = null)
        {
            return new Emoji(a.Value + zeroWidthJoiner.Value + b.Value, altDesc ?? $"{a.Desc}: {b.Desc}");
        }

        private static EmojiGroup Join(EmojiGroup A, Emoji b)
        {
            var temp = Join(A as Emoji, b);
            var alts = A.Alts.Select(a => Join(a, b)).ToArray();
            return new EmojiGroup(temp, alts);
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
            return new EmojiGroup(v, d, alts);
        }

        private static EmojiGroup Sex(Emoji person)
        {
            var man = Join(person, male);
            var woman = Join(person, female);

            return new EmojiGroup(person, person, man, woman);
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
            return new EmojiGroup(v, d, alts);
        }

        private static EmojiGroup Symbol(Emoji symbol, string name)
        {
            var j = new Emoji(symbol.Value, name);
            var men = Join(man.Alts[0], j);
            var women = Join(woman.Alts[0], j);
            return new EmojiGroup(symbol, symbol, men, women);
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

        public static readonly EmojiGroup gestures = new EmojiGroup(
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
        public static readonly EmojiGroup children = new EmojiGroup(child, child, boy, girl);


        public static readonly EmojiGroup blondes = SkinAndSex(char.ConvertFromUtf32(0x1F471), "Blond Person");
        public static readonly EmojiGroup person = Skin(char.ConvertFromUtf32(0x1F9D1), "Person",
            blondes,
            wearingTurban);

        public static readonly EmojiGroup beardedMan = Skin(char.ConvertFromUtf32(0x1F9D4), "Bearded Man");
        public static readonly Emoji manInSuitLevitating = new Emoji(char.ConvertFromUtf32(0x1F574) + emojiStyle.Value, "Man in Suit, Levitating");
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

        public static readonly EmojiGroup adults = new EmojiGroup(person.Value, "Adult", person, man, woman);

        public static readonly EmojiGroup olderPerson = Skin(char.ConvertFromUtf32(0x1F9D3), "Older Person");
        public static readonly EmojiGroup oldMan = Skin(char.ConvertFromUtf32(0x1F474), "Old Man");
        public static readonly EmojiGroup oldWoman = Skin(char.ConvertFromUtf32(0x1F475), "Old Woman");
        public static readonly EmojiGroup elderly = new EmojiGroup(olderPerson, olderPerson, oldMan, oldWoman);

        public static readonly Emoji medical = new Emoji("\u2695" + emojiStyle.Value, "Medical");
        public static readonly EmojiGroup healthCareWorkers = Symbol(medical, "Health Care");

        public static readonly Emoji graduationCap = new Emoji(char.ConvertFromUtf32(0x1F393), "Graduation Cap");
        public static readonly EmojiGroup students = Symbol(graduationCap, "Student");

        public static readonly Emoji school = new Emoji(char.ConvertFromUtf32(0x1F3EB), "School");
        public static readonly EmojiGroup teachers = Symbol(school, "Teacher");

        public static readonly Emoji balanceScale = new Emoji("\u2696" + emojiStyle.Value, "Balance Scale");
        public static readonly EmojiGroup judges = Symbol(balanceScale, "Judge");

        public static readonly Emoji sheafOfRice = new Emoji(char.ConvertFromUtf32(0x1F33E), "Sheaf of Rice");
        public static readonly EmojiGroup farmers = Symbol(sheafOfRice, "Farmer");

        public static readonly Emoji cooking = new Emoji(char.ConvertFromUtf32(0x1F373), "Cooking");
        public static readonly EmojiGroup cooks = Symbol(cooking, "Cook");

        public static readonly Emoji wrench = new Emoji(char.ConvertFromUtf32(0x1F527), "Wrench");
        public static readonly EmojiGroup mechanics = Symbol(wrench, "Mechanic");

        public static readonly Emoji factory = new Emoji(char.ConvertFromUtf32(0x1F3ED), "Factory");
        public static readonly EmojiGroup factoryWorkers = Symbol(factory, "Factory Worker");

        public static readonly Emoji briefcase = new Emoji(char.ConvertFromUtf32(0x1F4BC), "Briefcase");
        public static readonly EmojiGroup officeWorkers = Symbol(briefcase, "Office Worker");

        public static readonly Emoji fireEngine = new Emoji(char.ConvertFromUtf32(0x1F692), "Fire Engine");
        public static readonly EmojiGroup fireFighters = Symbol(fireEngine, "Fire Fighter");

        public static readonly Emoji rocket = new Emoji(char.ConvertFromUtf32(0x1F680), "Rocket");
        public static readonly EmojiGroup astronauts = Symbol(rocket, "Astronaut");

        public static readonly Emoji airplane = new Emoji("\u2708" + emojiStyle.Value, "Airplane");
        public static readonly EmojiGroup pilots = Symbol(airplane, "Pilot");

        public static readonly Emoji artistPalette = new Emoji(char.ConvertFromUtf32(0x1F3A8), "Artist Palette");
        public static readonly EmojiGroup artists = Symbol(artistPalette, "Artist");

        public static readonly Emoji microphone = new Emoji(char.ConvertFromUtf32(0x1F3A4), "Microphone");
        public static readonly EmojiGroup singers = Symbol(microphone, "Singer");

        public static readonly Emoji laptop = new Emoji(char.ConvertFromUtf32(0x1F4BB), "Laptop");
        public static readonly EmojiGroup technologists = Symbol(laptop, "Technologist");

        public static readonly Emoji microscope = new Emoji(char.ConvertFromUtf32(0x1F52C), "Microscope");
        public static readonly EmojiGroup scientists = Symbol(microscope, "Scientist");

        public static readonly Emoji crown = new Emoji(char.ConvertFromUtf32(0x1F451), "Crown");
        public static readonly EmojiGroup prince = Skin(char.ConvertFromUtf32(0x1F934), "Prince");
        public static readonly EmojiGroup princess = Skin(char.ConvertFromUtf32(0x1F478), "Princess");
        public static readonly EmojiGroup royalty = new EmojiGroup(crown, crown, prince, princess);

        public static readonly EmojiGroup roles = new EmojiGroup(
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

        public static readonly Emoji genie = new Emoji(char.ConvertFromUtf32(0x1F9DE), "Genie");
        public static readonly EmojiGroup genies = Sex(genie);
        public static readonly Emoji zombie = new Emoji(char.ConvertFromUtf32(0x1F9DF), "Zombie");
        public static readonly EmojiGroup zombies = Sex(zombie);

        public static readonly EmojiGroup fantasy = new EmojiGroup(
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

        public static readonly Emoji whiteCane = new Emoji(char.ConvertFromUtf32(0x1F9AF), "Probing Cane");
        public static readonly EmojiGroup withProbingCane = Symbol(whiteCane, "Probing");

        public static readonly Emoji motorizedWheelchair = new Emoji(char.ConvertFromUtf32(0x1F9BC), "Motorized Wheelchair");
        public static readonly EmojiGroup inMotorizedWheelchair = Symbol(motorizedWheelchair, "In Motorized Wheelchair");

        public static readonly Emoji manualWheelchair = new Emoji(char.ConvertFromUtf32(0x1F9BD), "Manual Wheelchair");
        public static readonly EmojiGroup inManualWheelchair = Symbol(manualWheelchair, "In Manual Wheelchair");


        public static readonly EmojiGroup manDancing = Skin(char.ConvertFromUtf32(0x1F57A), "Man Dancing");
        public static readonly EmojiGroup womanDancing = Skin(char.ConvertFromUtf32(0x1F483), "Woman Dancing");
        public static readonly EmojiGroup dancers = new EmojiGroup(manDancing.Value, "Dancing", manDancing, womanDancing);

        public static readonly EmojiGroup jugglers = SkinAndSex(char.ConvertFromUtf32(0x1F939), "Juggler");

        public static readonly EmojiGroup climbers = SkinAndSex(char.ConvertFromUtf32(0x1F9D7), "Climber");
        public static readonly Emoji fencer = new Emoji(char.ConvertFromUtf32(0x1F93A), "Fencer");
        public static readonly EmojiGroup jockeys = Skin(char.ConvertFromUtf32(0x1F3C7), "Jockey");
        public static readonly Emoji skier = new Emoji("\u26F7" + emojiStyle.Value, "Skier");
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
        public static readonly Emoji wrestler = new Emoji(char.ConvertFromUtf32(0x1F93C), "Wrestler");
        public static readonly EmojiGroup wrestlers = Sex(wrestler);
        public static readonly EmojiGroup waterPoloers = SkinAndSex(char.ConvertFromUtf32(0x1F93D), "Water Polo Player");
        public static readonly EmojiGroup handBallers = SkinAndSex(char.ConvertFromUtf32(0x1F93E), "Hand Baller");

        public static readonly EmojiGroup inMotion = new EmojiGroup(
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
        public static readonly EmojiGroup resting = new EmojiGroup(
            "Resting", "Depictions of people at rest",
            inLotusPosition,
            inBath,
            inBed,
            inSauna);

        public static readonly EmojiGroup babies = new EmojiGroup(baby, baby, cherub);
        public static readonly EmojiGroup people = new EmojiGroup(
            "People", "People",
            babies,
            children,
            adults,
            elderly);

        public static readonly EmojiGroup allPeople = new EmojiGroup(
            "All People", "All People",
            people,
            gestures,
            inMotion,
            resting,
            roles,
            fantasy);

        public static readonly Emoji ogre = new Emoji(char.ConvertFromUtf32(0x1F479), "Ogre");
        public static readonly Emoji goblin = new Emoji(char.ConvertFromUtf32(0x1F47A), "Goblin");
        public static readonly Emoji ghost = new Emoji(char.ConvertFromUtf32(0x1F47B), "Ghost");
        public static readonly Emoji alien = new Emoji(char.ConvertFromUtf32(0x1F47D), "Alien");
        public static readonly Emoji alienMonster = new Emoji(char.ConvertFromUtf32(0x1F47E), "Alien Monster");
        public static readonly Emoji angryFaceWithHorns = new Emoji(char.ConvertFromUtf32(0x1F47F), "Angry Face with Horns");
        public static readonly Emoji skull = new Emoji(char.ConvertFromUtf32(0x1F480), "Skull");
        public static readonly Emoji pileOfPoo = new Emoji(char.ConvertFromUtf32(0x1F4A9), "Pile of Poo");
        public static readonly Emoji grinningFace = new Emoji(char.ConvertFromUtf32(0x1F600), "Grinning Face");
        public static readonly Emoji beamingFaceWithSmilingEyes = new Emoji(char.ConvertFromUtf32(0x1F601), "Beaming Face with Smiling Eyes");
        public static readonly Emoji faceWithTearsOfJoy = new Emoji(char.ConvertFromUtf32(0x1F602), "Face with Tears of Joy");
        public static readonly Emoji grinningFaceWithBigEyes = new Emoji(char.ConvertFromUtf32(0x1F603), "Grinning Face with Big Eyes");
        public static readonly Emoji grinningFaceWithSmilingEyes = new Emoji(char.ConvertFromUtf32(0x1F604), "Grinning Face with Smiling Eyes");
        public static readonly Emoji grinningFaceWithSweat = new Emoji(char.ConvertFromUtf32(0x1F605), "Grinning Face with Sweat");
        public static readonly Emoji grinningSquitingFace = new Emoji(char.ConvertFromUtf32(0x1F606), "Grinning Squinting Face");
        public static readonly Emoji smillingFaceWithHalo = new Emoji(char.ConvertFromUtf32(0x1F607), "Smiling Face with Halo");
        public static readonly Emoji smilingFaceWithHorns = new Emoji(char.ConvertFromUtf32(0x1F608), "Smiling Face with Horns");
        public static readonly Emoji winkingFace = new Emoji(char.ConvertFromUtf32(0x1F609), "Winking Face");
        public static readonly Emoji smilingFaceWithSmilingEyes = new Emoji(char.ConvertFromUtf32(0x1F60A), "Smiling Face with Smiling Eyes");
        public static readonly Emoji faceSavoringFood = new Emoji(char.ConvertFromUtf32(0x1F60B), "Face Savoring Food");
        public static readonly Emoji relievedFace = new Emoji(char.ConvertFromUtf32(0x1F60C), "Relieved Face");
        public static readonly Emoji smilingFaceWithHeartEyes = new Emoji(char.ConvertFromUtf32(0x1F60D), "Smiling Face with Heart-Eyes");
        public static readonly Emoji smilingFaceWithSunglasses = new Emoji(char.ConvertFromUtf32(0x1F60E), "Smiling Face with Sunglasses");
        public static readonly Emoji smirkingFace = new Emoji(char.ConvertFromUtf32(0x1F60F), "Smirking Face");
        public static readonly Emoji neutralFace = new Emoji(char.ConvertFromUtf32(0x1F610), "Neutral Face");
        public static readonly Emoji expressionlessFace = new Emoji(char.ConvertFromUtf32(0x1F611), "Expressionless Face");
        public static readonly Emoji unamusedFace = new Emoji(char.ConvertFromUtf32(0x1F612), "Unamused Face");
        public static readonly Emoji downcastFaceWithSweat = new Emoji(char.ConvertFromUtf32(0x1F613), "Downcast Face with Sweat");
        public static readonly Emoji pensiveFace = new Emoji(char.ConvertFromUtf32(0x1F614), "Pensive Face");
        public static readonly Emoji confusedFace = new Emoji(char.ConvertFromUtf32(0x1F615), "Confused Face");
        public static readonly Emoji confoundedFace = new Emoji(char.ConvertFromUtf32(0x1F616), "Confounded Face");
        public static readonly Emoji kissingFace = new Emoji(char.ConvertFromUtf32(0x1F617), "Kissing Face");
        public static readonly Emoji faceBlowingAKiss = new Emoji(char.ConvertFromUtf32(0x1F618), "Face Blowing a Kiss");
        public static readonly Emoji kissingFaceWithSmilingEyes = new Emoji(char.ConvertFromUtf32(0x1F619), "Kissing Face with Smiling Eyes");
        public static readonly Emoji kissingFaceWithClosedEyes = new Emoji(char.ConvertFromUtf32(0x1F61A), "Kissing Face with Closed Eyes");
        public static readonly Emoji faceWithTongue = new Emoji(char.ConvertFromUtf32(0x1F61B), "Face with Tongue");
        public static readonly Emoji winkingFaceWithTongue = new Emoji(char.ConvertFromUtf32(0x1F61C), "Winking Face with Tongue");
        public static readonly Emoji squintingFaceWithTongue = new Emoji(char.ConvertFromUtf32(0x1F61D), "Squinting Face with Tongue");
        public static readonly Emoji disappointedFace = new Emoji(char.ConvertFromUtf32(0x1F61E), "Disappointed Face");
        public static readonly Emoji worriedFace = new Emoji(char.ConvertFromUtf32(0x1F61F), "Worried Face");
        public static readonly Emoji angryFace = new Emoji(char.ConvertFromUtf32(0x1F620), "Angry Face");
        public static readonly Emoji poutingFace = new Emoji(char.ConvertFromUtf32(0x1F621), "Pouting Face");
        public static readonly Emoji cryingFace = new Emoji(char.ConvertFromUtf32(0x1F622), "Crying Face");
        public static readonly Emoji perseveringFace = new Emoji(char.ConvertFromUtf32(0x1F623), "Persevering Face");
        public static readonly Emoji faceWithSteamFromNose = new Emoji(char.ConvertFromUtf32(0x1F624), "Face with Steam From Nose");
        public static readonly Emoji sadButRelievedFace = new Emoji(char.ConvertFromUtf32(0x1F625), "Sad but Relieved Face");
        public static readonly Emoji frowningFaceWithOpenMouth = new Emoji(char.ConvertFromUtf32(0x1F626), "Frowning Face with Open Mouth");
        public static readonly Emoji anguishedFace = new Emoji(char.ConvertFromUtf32(0x1F627), "Anguished Face");
        public static readonly Emoji fearfulFace = new Emoji(char.ConvertFromUtf32(0x1F628), "Fearful Face");
        public static readonly Emoji wearyFace = new Emoji(char.ConvertFromUtf32(0x1F629), "Weary Face");
        public static readonly Emoji sleepyFace = new Emoji(char.ConvertFromUtf32(0x1F62A), "Sleepy Face");
        public static readonly Emoji tiredFace = new Emoji(char.ConvertFromUtf32(0x1F62B), "Tired Face");
        public static readonly Emoji grimacingFace = new Emoji(char.ConvertFromUtf32(0x1F62C), "Grimacing Face");
        public static readonly Emoji loudlyCryingFace = new Emoji(char.ConvertFromUtf32(0x1F62D), "Loudly Crying Face");
        public static readonly Emoji faceWithOpenMouth = new Emoji(char.ConvertFromUtf32(0x1F62E), "Face with Open Mouth");
        public static readonly Emoji hushedFace = new Emoji(char.ConvertFromUtf32(0x1F62F), "Hushed Face");
        public static readonly Emoji anxiousFaceWithSweat = new Emoji(char.ConvertFromUtf32(0x1F630), "Anxious Face with Sweat");
        public static readonly Emoji faceScreamingInFear = new Emoji(char.ConvertFromUtf32(0x1F631), "Face Screaming in Fear");
        public static readonly Emoji astonishedFace = new Emoji(char.ConvertFromUtf32(0x1F632), "Astonished Face");
        public static readonly Emoji flushedFace = new Emoji(char.ConvertFromUtf32(0x1F633), "Flushed Face");
        public static readonly Emoji sleepingFace = new Emoji(char.ConvertFromUtf32(0x1F634), "Sleeping Face");
        public static readonly Emoji dizzyFace = new Emoji(char.ConvertFromUtf32(0x1F635), "Dizzy Face");
        public static readonly Emoji faceWithoutMouth = new Emoji(char.ConvertFromUtf32(0x1F636), "Face Without Mouth");
        public static readonly Emoji faceWithMedicalMask = new Emoji(char.ConvertFromUtf32(0x1F637), "Face with Medical Mask");
        public static readonly Emoji grinningCatWithSmilingEyes = new Emoji(char.ConvertFromUtf32(0x1F638), "Grinning Cat with Smiling Eyes");
        public static readonly Emoji catWithTearsOfJoy = new Emoji(char.ConvertFromUtf32(0x1F639), "Cat with Tears of Joy");
        public static readonly Emoji grinningCat = new Emoji(char.ConvertFromUtf32(0x1F63A), "Grinning Cat");
        public static readonly Emoji smilingCatWithHeartEyes = new Emoji(char.ConvertFromUtf32(0x1F63B), "Smiling Cat with Heart-Eyes");
        public static readonly Emoji catWithWrySmile = new Emoji(char.ConvertFromUtf32(0x1F63C), "Cat with Wry Smile");
        public static readonly Emoji kissingCat = new Emoji(char.ConvertFromUtf32(0x1F63D), "Kissing Cat");
        public static readonly Emoji poutingCat = new Emoji(char.ConvertFromUtf32(0x1F63E), "Pouting Cat");
        public static readonly Emoji cryingCat = new Emoji(char.ConvertFromUtf32(0x1F63F), "Crying Cat");
        public static readonly Emoji wearyCat = new Emoji(char.ConvertFromUtf32(0x1F640), "Weary Cat");
        public static readonly Emoji slightlyFrowningFace = new Emoji(char.ConvertFromUtf32(0x1F641), "Slightly Frowning Face");
        public static readonly Emoji slightlySmilingFace = new Emoji(char.ConvertFromUtf32(0x1F642), "Slightly Smiling Face");
        public static readonly Emoji updisdeDownFace = new Emoji(char.ConvertFromUtf32(0x1F643), "Upside-Down Face");
        public static readonly Emoji faceWithRollingEyes = new Emoji(char.ConvertFromUtf32(0x1F644), "Face with Rolling Eyes");
        public static readonly Emoji seeNoEvilMonkey = new Emoji(char.ConvertFromUtf32(0x1F648), "See-No-Evil Monkey");
        public static readonly Emoji hearNoEvilMonkey = new Emoji(char.ConvertFromUtf32(0x1F649), "Hear-No-Evil Monkey");
        public static readonly Emoji speakNoEvilMonkey = new Emoji(char.ConvertFromUtf32(0x1F64A), "Speak-No-Evil Monkey");
        public static readonly Emoji zipperMouthFace = new Emoji(char.ConvertFromUtf32(0x1F910), "Zipper-Mouth Face");
        public static readonly Emoji moneyMouthFace = new Emoji(char.ConvertFromUtf32(0x1F911), "Money-Mouth Face");
        public static readonly Emoji faceWithThermometer = new Emoji(char.ConvertFromUtf32(0x1F912), "Face with Thermometer");
        public static readonly Emoji nerdFace = new Emoji(char.ConvertFromUtf32(0x1F913), "Nerd Face");
        public static readonly Emoji thinkingFace = new Emoji(char.ConvertFromUtf32(0x1F914), "Thinking Face");
        public static readonly Emoji faceWithHeadBandage = new Emoji(char.ConvertFromUtf32(0x1F915), "Face with Head-Bandage");
        public static readonly Emoji robot = new Emoji(char.ConvertFromUtf32(0x1F916), "Robot");
        public static readonly Emoji huggingFace = new Emoji(char.ConvertFromUtf32(0x1F917), "Hugging Face");
        public static readonly Emoji cowboyHatFace = new Emoji(char.ConvertFromUtf32(0x1F920), "Cowboy Hat Face");
        public static readonly Emoji clownFace = new Emoji(char.ConvertFromUtf32(0x1F921), "Clown Face");
        public static readonly Emoji nauseatedFace = new Emoji(char.ConvertFromUtf32(0x1F922), "Nauseated Face");
        public static readonly Emoji rollingOnTheFloorLaughing = new Emoji(char.ConvertFromUtf32(0x1F923), "Rolling on the Floor Laughing");
        public static readonly Emoji droolingFace = new Emoji(char.ConvertFromUtf32(0x1F924), "Drooling Face");
        public static readonly Emoji lyingFace = new Emoji(char.ConvertFromUtf32(0x1F925), "Lying Face");
        public static readonly Emoji sneezingFace = new Emoji(char.ConvertFromUtf32(0x1F927), "Sneezing Face");
        public static readonly Emoji faceWithRaisedEyebrow = new Emoji(char.ConvertFromUtf32(0x1F928), "Face with Raised Eyebrow");
        public static readonly Emoji starStruck = new Emoji(char.ConvertFromUtf32(0x1F929), "Star-Struck");
        public static readonly Emoji zanyFace = new Emoji(char.ConvertFromUtf32(0x1F92A), "Zany Face");
        public static readonly Emoji shushingFace = new Emoji(char.ConvertFromUtf32(0x1F92B), "Shushing Face");
        public static readonly Emoji faceWithSymbolsOnMouth = new Emoji(char.ConvertFromUtf32(0x1F92C), "Face with Symbols on Mouth");
        public static readonly Emoji faceWithHandOverMouth = new Emoji(char.ConvertFromUtf32(0x1F92D), "Face with Hand Over Mouth");
        public static readonly Emoji faceVomitting = new Emoji(char.ConvertFromUtf32(0x1F92E), "Face Vomiting");
        public static readonly Emoji explodingHead = new Emoji(char.ConvertFromUtf32(0x1F92F), "Exploding Head");
        public static readonly Emoji smilingFaceWithHearts = new Emoji(char.ConvertFromUtf32(0x1F970), "Smiling Face with Hearts");
        public static readonly Emoji yawningFace = new Emoji(char.ConvertFromUtf32(0x1F971), "Yawning Face");
        public static readonly Emoji smilingFaceWithTear = new Emoji(char.ConvertFromUtf32(0x1F972), "Smiling Face with Tear");
        public static readonly Emoji partyingFace = new Emoji(char.ConvertFromUtf32(0x1F973), "Partying Face");
        public static readonly Emoji woozyFace = new Emoji(char.ConvertFromUtf32(0x1F974), "Woozy Face");
        public static readonly Emoji hotFace = new Emoji(char.ConvertFromUtf32(0x1F975), "Hot Face");
        public static readonly Emoji coldFace = new Emoji(char.ConvertFromUtf32(0x1F976), "Cold Face");
        public static readonly Emoji disguisedFace = new Emoji(char.ConvertFromUtf32(0x1F978), "Disguised Face");
        public static readonly Emoji pleadingFace = new Emoji(char.ConvertFromUtf32(0x1F97A), "Pleading Face");
        public static readonly Emoji faceWithMonocle = new Emoji(char.ConvertFromUtf32(0x1F9D0), "Face with Monocle");
        public static readonly Emoji skullAndCrossbones = new Emoji("\u2620" + emojiStyle.Value, "Skull and Crossbones");
        public static readonly Emoji frowningFace = new Emoji("\u2639" + emojiStyle.Value, "Frowning Face");
        public static readonly Emoji smilingFace = new Emoji("\u263A" + emojiStyle.Value, "Smiling Face");
        public static readonly Emoji speakingHead = new Emoji(char.ConvertFromUtf32(0x1F5E3) + emojiStyle.Value, "Speaking Head");
        public static readonly Emoji bust = new Emoji(char.ConvertFromUtf32(0x1F464), "Bust in Silhouette");
        public static readonly EmojiGroup faces = new EmojiGroup(
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

        public static readonly Emoji kissMark = new Emoji(char.ConvertFromUtf32(0x1F48B), "Kiss Mark");
        public static readonly Emoji loveLetter = new Emoji(char.ConvertFromUtf32(0x1F48C), "Love Letter");
        public static readonly Emoji beatingHeart = new Emoji(char.ConvertFromUtf32(0x1F493), "Beating Heart");
        public static readonly Emoji brokenHeart = new Emoji(char.ConvertFromUtf32(0x1F494), "Broken Heart");
        public static readonly Emoji twoHearts = new Emoji(char.ConvertFromUtf32(0x1F495), "Two Hearts");
        public static readonly Emoji sparklingHeart = new Emoji(char.ConvertFromUtf32(0x1F496), "Sparkling Heart");
        public static readonly Emoji growingHeart = new Emoji(char.ConvertFromUtf32(0x1F497), "Growing Heart");
        public static readonly Emoji heartWithArrow = new Emoji(char.ConvertFromUtf32(0x1F498), "Heart with Arrow");
        public static readonly Emoji blueHeart = new Emoji(char.ConvertFromUtf32(0x1F499), "Blue Heart");
        public static readonly Emoji greenHeart = new Emoji(char.ConvertFromUtf32(0x1F49A), "Green Heart");
        public static readonly Emoji yellowHeart = new Emoji(char.ConvertFromUtf32(0x1F49B), "Yellow Heart");
        public static readonly Emoji purpleHeart = new Emoji(char.ConvertFromUtf32(0x1F49C), "Purple Heart");
        public static readonly Emoji heartWithRibbon = new Emoji(char.ConvertFromUtf32(0x1F49D), "Heart with Ribbon");
        public static readonly Emoji revolvingHearts = new Emoji(char.ConvertFromUtf32(0x1F49E), "Revolving Hearts");
        public static readonly Emoji heartDecoration = new Emoji(char.ConvertFromUtf32(0x1F49F), "Heart Decoration");
        public static readonly Emoji blackHeart = new Emoji(char.ConvertFromUtf32(0x1F5A4), "Black Heart");
        public static readonly Emoji whiteHeart = new Emoji(char.ConvertFromUtf32(0x1F90D), "White Heart");
        public static readonly Emoji brownHeart = new Emoji(char.ConvertFromUtf32(0x1F90E), "Brown Heart");
        public static readonly Emoji orangeHeart = new Emoji(char.ConvertFromUtf32(0x1F9E1), "Orange Heart");
        public static readonly Emoji heartExclamation = new Emoji("\u2763" + emojiStyle.Value, "Heart Exclamation");
        public static readonly Emoji redHeart = new Emoji("\u2764" + emojiStyle.Value, "Red Heart");
        public static readonly EmojiGroup love = new EmojiGroup(
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

        public static readonly Emoji angerSymbol = new Emoji(char.ConvertFromUtf32(0x1F4A2), "Anger Symbol");
        public static readonly Emoji bomb = new Emoji(char.ConvertFromUtf32(0x1F4A3), "Bomb");
        public static readonly Emoji zzz = new Emoji(char.ConvertFromUtf32(0x1F4A4), "Zzz");
        public static readonly Emoji collision = new Emoji(char.ConvertFromUtf32(0x1F4A5), "Collision");
        public static readonly Emoji sweatDroplets = new Emoji(char.ConvertFromUtf32(0x1F4A6), "Sweat Droplets");
        public static readonly Emoji dashingAway = new Emoji(char.ConvertFromUtf32(0x1F4A8), "Dashing Away");
        public static readonly Emoji dizzy = new Emoji(char.ConvertFromUtf32(0x1F4AB), "Dizzy");
        public static readonly Emoji speechBalloon = new Emoji(char.ConvertFromUtf32(0x1F4AC), "Speech Balloon");
        public static readonly Emoji thoughtBalloon = new Emoji(char.ConvertFromUtf32(0x1F4AD), "Thought Balloon");
        public static readonly Emoji hundredPoints = new Emoji(char.ConvertFromUtf32(0x1F4AF), "Hundred Points");
        public static readonly Emoji hole = new Emoji(char.ConvertFromUtf32(0x1F573) + emojiStyle.Value, "Hole");
        public static readonly Emoji leftSpeechBubble = new Emoji(char.ConvertFromUtf32(0x1F5E8) + emojiStyle.Value, "Left Speech Bubble");
        public static readonly Emoji rightSpeechBubble = new Emoji(char.ConvertFromUtf32(0x1F5E9) + emojiStyle.Value, "Right Speech Bubble");
        public static readonly Emoji conversationBubbles2 = new Emoji(char.ConvertFromUtf32(0x1F5EA) + emojiStyle.Value, "Conversation Bubbles 2");
        public static readonly Emoji conversationBubbles3 = new Emoji(char.ConvertFromUtf32(0x1F5EB) + emojiStyle.Value, "Conversation Bubbles 3");
        public static readonly Emoji leftThoughtBubble = new Emoji(char.ConvertFromUtf32(0x1F5EC) + emojiStyle.Value, "Left Thought Bubble");
        public static readonly Emoji rightThoughtBubble = new Emoji(char.ConvertFromUtf32(0x1F5ED) + emojiStyle.Value, "Right Thought Bubble");
        public static readonly Emoji leftAngerBubble = new Emoji(char.ConvertFromUtf32(0x1F5EE) + emojiStyle.Value, "Left Anger Bubble");
        public static readonly Emoji rightAngerBubble = new Emoji(char.ConvertFromUtf32(0x1F5EF) + emojiStyle.Value, "Right Anger Bubble");
        public static readonly Emoji angerBubble = new Emoji(char.ConvertFromUtf32(0x1F5F0) + emojiStyle.Value, "Anger Bubble");
        public static readonly Emoji angerBubbleLightningBolt = new Emoji(char.ConvertFromUtf32(0x1F5F1) + emojiStyle.Value, "Anger Bubble Lightning");
        public static readonly Emoji lightningBolt = new Emoji(char.ConvertFromUtf32(0x1F5F2) + emojiStyle.Value, "Lightning Bolt");

        public static readonly EmojiGroup cartoon = new EmojiGroup(
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

        public static readonly Emoji backhandIndexPointingUp = new Emoji(char.ConvertFromUtf32(0x1F446), "Backhand Index Pointing Up");
        public static readonly Emoji backhandIndexPointingDown = new Emoji(char.ConvertFromUtf32(0x1F447), "Backhand Index Pointing Down");
        public static readonly Emoji backhandIndexPointingLeft = new Emoji(char.ConvertFromUtf32(0x1F448), "Backhand Index Pointing Left");
        public static readonly Emoji backhandIndexPointingRight = new Emoji(char.ConvertFromUtf32(0x1F449), "Backhand Index Pointing Right");
        public static readonly Emoji oncomingFist = new Emoji(char.ConvertFromUtf32(0x1F44A), "Oncoming Fist");
        public static readonly Emoji wavingHand = new Emoji(char.ConvertFromUtf32(0x1F44B), "Waving Hand");
        public static readonly Emoji okHand = new Emoji(char.ConvertFromUtf32(0x1F58F), "OK Hand");
        public static readonly Emoji thumbsUp = new Emoji(char.ConvertFromUtf32(0x1F44D), "Thumbs Up");
        public static readonly Emoji thumbsDown = new Emoji(char.ConvertFromUtf32(0x1F44E), "Thumbs Down");
        public static readonly Emoji clappingHands = new Emoji(char.ConvertFromUtf32(0x1F44F), "Clapping Hands");
        public static readonly Emoji openHands = new Emoji(char.ConvertFromUtf32(0x1F450), "Open Hands");
        public static readonly Emoji nailPolish = new Emoji(char.ConvertFromUtf32(0x1F485), "Nail Polish");
        public static readonly Emoji handsWithFingersSplayed = new Emoji(char.ConvertFromUtf32(0x1F590) + emojiStyle.Value, "Hand with Fingers Splayed");
        public static readonly Emoji handsWithFingersSplayed2 = new Emoji(char.ConvertFromUtf32(0x1F591) + emojiStyle.Value, "Hand with Fingers Splayed 2");
        public static readonly Emoji thumbsUp2 = new Emoji(char.ConvertFromUtf32(0x1F592), "Thumbs Up 2");
        public static readonly Emoji thumbsDown2 = new Emoji(char.ConvertFromUtf32(0x1F593), "Thumbs Down 2");
        public static readonly Emoji peaceFingers = new Emoji(char.ConvertFromUtf32(0x1F594), "Peace Fingers");
        public static readonly Emoji middleFinger = new Emoji(char.ConvertFromUtf32(0x1F595), "Middle Finger");
        public static readonly Emoji vulcanSalute = new Emoji(char.ConvertFromUtf32(0x1F596), "Vulcan Salute");
        public static readonly Emoji handPointingDown = new Emoji(char.ConvertFromUtf32(0x1F597), "Hand Pointing Down");
        public static readonly Emoji handPointingLeft = new Emoji(char.ConvertFromUtf32(0x1F598), "Hand Pointing Left");
        public static readonly Emoji handPointingRight = new Emoji(char.ConvertFromUtf32(0x1F599), "Hand Pointing Right");
        public static readonly Emoji handPointingLeft2 = new Emoji(char.ConvertFromUtf32(0x1F59A), "Hand Pointing Left 2");
        public static readonly Emoji handPointingRight2 = new Emoji(char.ConvertFromUtf32(0x1F59B), "Hand Pointing Right 2");
        public static readonly Emoji indexPointingLeft = new Emoji(char.ConvertFromUtf32(0x1F59C), "Index Pointing Left");
        public static readonly Emoji indexPointingRight = new Emoji(char.ConvertFromUtf32(0x1F59D), "Index Pointing Right");
        public static readonly Emoji indexPointingUp = new Emoji(char.ConvertFromUtf32(0x1F59E), "Index Pointing Up");
        public static readonly Emoji indexPointingDown = new Emoji(char.ConvertFromUtf32(0x1F59F), "Index Pointing Down");
        public static readonly Emoji indexPointingUp2 = new Emoji(char.ConvertFromUtf32(0x1F5A0), "Index Pointing Up 2");
        public static readonly Emoji indexPointingDown2 = new Emoji(char.ConvertFromUtf32(0x1F5A1), "Index Pointing Down 2");
        public static readonly Emoji indexPointingUp3 = new Emoji(char.ConvertFromUtf32(0x1F5A2), "Index Pointing Up 3");
        public static readonly Emoji indexPointingDown3 = new Emoji(char.ConvertFromUtf32(0x1F5A3), "Index Pointing Down 3");
        public static readonly Emoji raisingHands = new Emoji(char.ConvertFromUtf32(0x1F64C), "Raising Hands");
        public static readonly Emoji foldedHands = new Emoji(char.ConvertFromUtf32(0x1F64F), "Folded Hands");
        public static readonly Emoji pinchedFingers = new Emoji(char.ConvertFromUtf32(0x1F90C), "Pinched Fingers");
        public static readonly Emoji pinchingHand = new Emoji(char.ConvertFromUtf32(0x1F90F), "Pinching Hand");
        public static readonly Emoji signOfTheHorns = new Emoji(char.ConvertFromUtf32(0x1F918), "Sign of the Horns");
        public static readonly Emoji callMeHand = new Emoji(char.ConvertFromUtf32(0x1F919), "Call Me Hand");
        public static readonly Emoji rasiedBackOfHand = new Emoji(char.ConvertFromUtf32(0x1F91A), "Raised Back of Hand");
        public static readonly Emoji leftFacingFist = new Emoji(char.ConvertFromUtf32(0x1F91B), "Left-Facing Fist");
        public static readonly Emoji rightFacingFist = new Emoji(char.ConvertFromUtf32(0x1F91C), "Right-Facing Fist");
        public static readonly Emoji handshake = new Emoji(char.ConvertFromUtf32(0x1F91D), "Handshake");
        public static readonly Emoji crossedFingers = new Emoji(char.ConvertFromUtf32(0x1F91E), "Crossed Fingers");
        public static readonly Emoji loveYouGesture = new Emoji(char.ConvertFromUtf32(0x1F91F), "Love-You Gesture");
        public static readonly Emoji palmsUpTogether = new Emoji(char.ConvertFromUtf32(0x1F932), "Palms Up Together");
        public static readonly Emoji indexPointingUp4 = new Emoji("\u261D" + emojiStyle.Value, "Index Pointing Up 4");
        public static readonly Emoji raisedFist = new Emoji("\u270A", "Raised Fist");
        public static readonly Emoji raisedHand = new Emoji("\u270B", "Raised Hand");
        public static readonly Emoji victoryHand = new Emoji("\u270C" + emojiStyle.Value, "Victory Hand");
        public static readonly Emoji writingHand = new Emoji("\u270D" + emojiStyle.Value, "Writing Hand");
        public static readonly EmojiGroup hands = new EmojiGroup(
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

        public static readonly EmojiGroup bodyParts = new EmojiGroup(
            "Body Parts", "General body parts",
            new Emoji(char.ConvertFromUtf32(0x1F440), "Eyes"),
            new Emoji(char.ConvertFromUtf32(0x1F441) + emojiStyle.Value, "Eye"),
            new Emoji(char.ConvertFromUtf32(0x1F441) + emojiStyle.Value + zeroWidthJoiner.Value + char.ConvertFromUtf32(0x1F5E8) + emojiStyle.Value, "Eye in Speech Bubble"),
            new Emoji(char.ConvertFromUtf32(0x1F442), "Ear"),
            new Emoji(char.ConvertFromUtf32(0x1F443), "Nose"),
            new Emoji(char.ConvertFromUtf32(0x1F444), "Mouth"),
            new Emoji(char.ConvertFromUtf32(0x1F445), "Tongue"),
            new Emoji(char.ConvertFromUtf32(0x1F4AA), "Flexed Biceps"),
            new Emoji(char.ConvertFromUtf32(0x1F933), "Selfie"),
            new Emoji(char.ConvertFromUtf32(0x1F9B4), "Bone"),
            new Emoji(char.ConvertFromUtf32(0x1F9B5), "Leg"),
            new Emoji(char.ConvertFromUtf32(0x1F9B6), "Foot"),
            new Emoji(char.ConvertFromUtf32(0x1F9B7), "Tooth"),
            new Emoji(char.ConvertFromUtf32(0x1F9BB), "Ear with Hearing Aid"),
            new Emoji(char.ConvertFromUtf32(0x1F9BE), "Mechanical Arm"),
            new Emoji(char.ConvertFromUtf32(0x1F9BF), "Mechanical Leg"),
            new Emoji(char.ConvertFromUtf32(0x1FAC0), "Anatomical Heart"),
            new Emoji(char.ConvertFromUtf32(0x1FAC1), "Lungs"),
            new Emoji(char.ConvertFromUtf32(0x1F9E0), "Brain"));

        public static readonly Emoji whiteFlower = new Emoji(char.ConvertFromUtf32(0x1F4AE), "White Flower");
        public static readonly EmojiGroup plants = new EmojiGroup(
            "Plants", "Flowers, trees, and things",
            new Emoji(char.ConvertFromUtf32(0x1F331), "Seedling"),
            new Emoji(char.ConvertFromUtf32(0x1F332), "Evergreen Tree"),
            new Emoji(char.ConvertFromUtf32(0x1F333), "Deciduous Tree"),
            new Emoji(char.ConvertFromUtf32(0x1F334), "Palm Tree"),
            new Emoji(char.ConvertFromUtf32(0x1F335), "Cactus"),
            new Emoji(char.ConvertFromUtf32(0x1F337), "Tulip"),
            new Emoji(char.ConvertFromUtf32(0x1F338), "Cherry Blossom"),
            new Emoji(char.ConvertFromUtf32(0x1F339), "Rose"),
            new Emoji(char.ConvertFromUtf32(0x1F33A), "Hibiscus"),
            new Emoji(char.ConvertFromUtf32(0x1F33B), "Sunflower"),
            new Emoji(char.ConvertFromUtf32(0x1F33C), "Blossom"),
            sheafOfRice,
            new Emoji(char.ConvertFromUtf32(0x1F33F), "Herb"),
            new Emoji(char.ConvertFromUtf32(0x1F340), "Four Leaf Clover"),
            new Emoji(char.ConvertFromUtf32(0x1F341), "Maple Leaf"),
            new Emoji(char.ConvertFromUtf32(0x1F342), "Fallen Leaf"),
            new Emoji(char.ConvertFromUtf32(0x1F343), "Leaf Fluttering in Wind"),
            new Emoji(char.ConvertFromUtf32(0x1F3F5) + emojiStyle.Value, "Rosette"),
            new Emoji(char.ConvertFromUtf32(0x1F490), "Bouquet"),
            whiteFlower,
            new Emoji(char.ConvertFromUtf32(0x1F940), "Wilted Flower"),
            new Emoji(char.ConvertFromUtf32(0x1FAB4), "Potted Plant"),
            new Emoji("\u2618" + emojiStyle.Value, "Shamrock"));

        public static readonly Emoji banana = new Emoji(char.ConvertFromUtf32(0x1F34C), "Banana");
        public static readonly EmojiGroup food = new EmojiGroup(
            "Food", "Food, drink, and utensils",
            new Emoji(char.ConvertFromUtf32(0x1F32D), "Hot Dog"),
            new Emoji(char.ConvertFromUtf32(0x1F32E), "Taco"),
            new Emoji(char.ConvertFromUtf32(0x1F32F), "Burrito"),
            new Emoji(char.ConvertFromUtf32(0x1F330), "Chestnut"),
            new Emoji(char.ConvertFromUtf32(0x1F336) + emojiStyle.Value, "Hot Pepper"),
            new Emoji(char.ConvertFromUtf32(0x1F33D), "Ear of Corn"),
            new Emoji(char.ConvertFromUtf32(0x1F344), "Mushroom"),
            new Emoji(char.ConvertFromUtf32(0x1F345), "Tomato"),
            new Emoji(char.ConvertFromUtf32(0x1F346), "Eggplant"),
            new Emoji(char.ConvertFromUtf32(0x1F347), "Grapes"),
            new Emoji(char.ConvertFromUtf32(0x1F348), "Melon"),
            new Emoji(char.ConvertFromUtf32(0x1F349), "Watermelon"),
            new Emoji(char.ConvertFromUtf32(0x1F34A), "Tangerine"),
            new Emoji(char.ConvertFromUtf32(0x1F34B), "Lemon"),
            banana,
            new Emoji(char.ConvertFromUtf32(0x1F34D), "Pineapple"),
            new Emoji(char.ConvertFromUtf32(0x1F34E), "Red Apple"),
            new Emoji(char.ConvertFromUtf32(0x1F34F), "Green Apple"),
            new Emoji(char.ConvertFromUtf32(0x1F350), "Pear"),
            new Emoji(char.ConvertFromUtf32(0x1F351), "Peach"),
            new Emoji(char.ConvertFromUtf32(0x1F352), "Cherries"),
            new Emoji(char.ConvertFromUtf32(0x1F353), "Strawberry"),
            new Emoji(char.ConvertFromUtf32(0x1F354), "Hamburger"),
            new Emoji(char.ConvertFromUtf32(0x1F355), "Pizza"),
            new Emoji(char.ConvertFromUtf32(0x1F356), "Meat on Bone"),
            new Emoji(char.ConvertFromUtf32(0x1F357), "Poultry Leg"),
            new Emoji(char.ConvertFromUtf32(0x1F358), "Rice Cracker"),
            new Emoji(char.ConvertFromUtf32(0x1F359), "Rice Ball"),
            new Emoji(char.ConvertFromUtf32(0x1F35A), "Cooked Rice"),
            new Emoji(char.ConvertFromUtf32(0x1F35B), "Curry Rice"),
            new Emoji(char.ConvertFromUtf32(0x1F35C), "Steaming Bowl"),
            new Emoji(char.ConvertFromUtf32(0x1F35D), "Spaghetti"),
            new Emoji(char.ConvertFromUtf32(0x1F35E), "Bread"),
            new Emoji(char.ConvertFromUtf32(0x1F35F), "French Fries"),
            new Emoji(char.ConvertFromUtf32(0x1F360), "Roasted Sweet Potato"),
            new Emoji(char.ConvertFromUtf32(0x1F361), "Dango"),
            new Emoji(char.ConvertFromUtf32(0x1F362), "Oden"),
            new Emoji(char.ConvertFromUtf32(0x1F363), "Sushi"),
            new Emoji(char.ConvertFromUtf32(0x1F364), "Fried Shrimp"),
            new Emoji(char.ConvertFromUtf32(0x1F365), "Fish Cake with Swirl"),
            new Emoji(char.ConvertFromUtf32(0x1F371), "Bento Box"),
            new Emoji(char.ConvertFromUtf32(0x1F372), "Pot of Food"),
            cooking,
            new Emoji(char.ConvertFromUtf32(0x1F37F), "Popcorn"),
            new Emoji(char.ConvertFromUtf32(0x1F950), "Croissant"),
            new Emoji(char.ConvertFromUtf32(0x1F951), "Avocado"),
            new Emoji(char.ConvertFromUtf32(0x1F952), "Cucumber"),
            new Emoji(char.ConvertFromUtf32(0x1F953), "Bacon"),
            new Emoji(char.ConvertFromUtf32(0x1F954), "Potato"),
            new Emoji(char.ConvertFromUtf32(0x1F955), "Carrot"),
            new Emoji(char.ConvertFromUtf32(0x1F956), "Baguette Bread"),
            new Emoji(char.ConvertFromUtf32(0x1F957), "Green Salad"),
            new Emoji(char.ConvertFromUtf32(0x1F958), "Shallow Pan of Food"),
            new Emoji(char.ConvertFromUtf32(0x1F959), "Stuffed Flatbread"),
            new Emoji(char.ConvertFromUtf32(0x1F95A), "Egg"),
            new Emoji(char.ConvertFromUtf32(0x1F95C), "Peanuts"),
            new Emoji(char.ConvertFromUtf32(0x1F95D), "Kiwi Fruit"),
            new Emoji(char.ConvertFromUtf32(0x1F95E), "Pancakes"),
            new Emoji(char.ConvertFromUtf32(0x1F95F), "Dumpling"),
            new Emoji(char.ConvertFromUtf32(0x1F960), "Fortune Cookie"),
            new Emoji(char.ConvertFromUtf32(0x1F961), "Takeout Box"),
            new Emoji(char.ConvertFromUtf32(0x1F963), "Bowl with Spoon"),
            new Emoji(char.ConvertFromUtf32(0x1F965), "Coconut"),
            new Emoji(char.ConvertFromUtf32(0x1F966), "Broccoli"),
            new Emoji(char.ConvertFromUtf32(0x1F968), "Pretzel"),
            new Emoji(char.ConvertFromUtf32(0x1F969), "Cut of Meat"),
            new Emoji(char.ConvertFromUtf32(0x1F96A), "Sandwich"),
            new Emoji(char.ConvertFromUtf32(0x1F96B), "Canned Food"),
            new Emoji(char.ConvertFromUtf32(0x1F96C), "Leafy Green"),
            new Emoji(char.ConvertFromUtf32(0x1F96D), "Mango"),
            new Emoji(char.ConvertFromUtf32(0x1F96E), "Moon Cake"),
            new Emoji(char.ConvertFromUtf32(0x1F96F), "Bagel"),
            new Emoji(char.ConvertFromUtf32(0x1F980), "Crab"),
            new Emoji(char.ConvertFromUtf32(0x1F990), "Shrimp"),
            new Emoji(char.ConvertFromUtf32(0x1F991), "Squid"),
            new Emoji(char.ConvertFromUtf32(0x1F99E), "Lobster"),
            new Emoji(char.ConvertFromUtf32(0x1F9AA), "Oyster"),
            new Emoji(char.ConvertFromUtf32(0x1F9C0), "Cheese Wedge"),
            new Emoji(char.ConvertFromUtf32(0x1F9C2), "Salt"),
            new Emoji(char.ConvertFromUtf32(0x1F9C4), "Garlic"),
            new Emoji(char.ConvertFromUtf32(0x1F9C5), "Onion"),
            new Emoji(char.ConvertFromUtf32(0x1F9C6), "Falafel"),
            new Emoji(char.ConvertFromUtf32(0x1F9C7), "Waffle"),
            new Emoji(char.ConvertFromUtf32(0x1F9C8), "Butter"),
            new Emoji(char.ConvertFromUtf32(0x1FAD0), "Blueberries"),
            new Emoji(char.ConvertFromUtf32(0x1FAD1), "Bell Pepper"),
            new Emoji(char.ConvertFromUtf32(0x1FAD2), "Olive"),
            new Emoji(char.ConvertFromUtf32(0x1FAD3), "Flatbread"),
            new Emoji(char.ConvertFromUtf32(0x1FAD4), "Tamale"),
            new Emoji(char.ConvertFromUtf32(0x1FAD5), "Fondue"),
            new Emoji(char.ConvertFromUtf32(0x1F366), "Soft Ice Cream"),
            new Emoji(char.ConvertFromUtf32(0x1F367), "Shaved Ice"),
            new Emoji(char.ConvertFromUtf32(0x1F368), "Ice Cream"),
            new Emoji(char.ConvertFromUtf32(0x1F369), "Doughnut"),
            new Emoji(char.ConvertFromUtf32(0x1F36A), "Cookie"),
            new Emoji(char.ConvertFromUtf32(0x1F36B), "Chocolate Bar"),
            new Emoji(char.ConvertFromUtf32(0x1F36C), "Candy"),
            new Emoji(char.ConvertFromUtf32(0x1F36D), "Lollipop"),
            new Emoji(char.ConvertFromUtf32(0x1F36E), "Custard"),
            new Emoji(char.ConvertFromUtf32(0x1F36F), "Honey Pot"),
            new Emoji(char.ConvertFromUtf32(0x1F370), "Shortcake"),
            new Emoji(char.ConvertFromUtf32(0x1F382), "Birthday Cake"),
            new Emoji(char.ConvertFromUtf32(0x1F967), "Pie"),
            new Emoji(char.ConvertFromUtf32(0x1F9C1), "Cupcake"),
            new Emoji(char.ConvertFromUtf32(0x1F375), "Teacup Without Handle"),
            new Emoji(char.ConvertFromUtf32(0x1F376), "Sake"),
            new Emoji(char.ConvertFromUtf32(0x1F377), "Wine Glass"),
            new Emoji(char.ConvertFromUtf32(0x1F378), "Cocktail Glass"),
            new Emoji(char.ConvertFromUtf32(0x1F379), "Tropical Drink"),
            new Emoji(char.ConvertFromUtf32(0x1F37A), "Beer Mug"),
            new Emoji(char.ConvertFromUtf32(0x1F37B), "Clinking Beer Mugs"),
            new Emoji(char.ConvertFromUtf32(0x1F37C), "Baby Bottle"),
            new Emoji(char.ConvertFromUtf32(0x1F37E), "Bottle with Popping Cork"),
            new Emoji(char.ConvertFromUtf32(0x1F942), "Clinking Glasses"),
            new Emoji(char.ConvertFromUtf32(0x1F943), "Tumbler Glass"),
            new Emoji(char.ConvertFromUtf32(0x1F95B), "Glass of Milk"),
            new Emoji(char.ConvertFromUtf32(0x1F964), "Cup with Straw"),
            new Emoji(char.ConvertFromUtf32(0x1F9C3), "Beverage Box"),
            new Emoji(char.ConvertFromUtf32(0x1F9C9), "Mate"),
            new Emoji(char.ConvertFromUtf32(0x1F9CA), "Ice"),
            new Emoji(char.ConvertFromUtf32(0x1F9CB), "Bubble Tea"),
            new Emoji(char.ConvertFromUtf32(0x1FAD6), "Teapot"),
            new Emoji("\u2615", "Hot Beverage"),
            new Emoji(char.ConvertFromUtf32(0x1F374), "Fork and Knife"),
            new Emoji(char.ConvertFromUtf32(0x1F37D) + emojiStyle.Value, "Fork and Knife with Plate"),
            new Emoji(char.ConvertFromUtf32(0x1F3FA), "Amphora"),
            new Emoji(char.ConvertFromUtf32(0x1F52A), "Kitchen Knife"),
            new Emoji(char.ConvertFromUtf32(0x1F944), "Spoon"),
            new Emoji(char.ConvertFromUtf32(0x1F962), "Chopsticks"));

        public static readonly Emoji motorcycle = new Emoji(char.ConvertFromUtf32(0x1F3CD) + emojiStyle.Value, "Motorcycle");
        public static readonly Emoji racingCar = new Emoji(char.ConvertFromUtf32(0x1F3CE) + emojiStyle.Value, "Racing Car");
        public static readonly Emoji seat = new Emoji(char.ConvertFromUtf32(0x1F4BA), "Seat");
        public static readonly Emoji helicopter = new Emoji(char.ConvertFromUtf32(0x1F681), "Helicopter");
        public static readonly Emoji locomotive = new Emoji(char.ConvertFromUtf32(0x1F682), "Locomotive");
        public static readonly Emoji railwayCar = new Emoji(char.ConvertFromUtf32(0x1F683), "Railway Car");
        public static readonly Emoji highspeedTrain = new Emoji(char.ConvertFromUtf32(0x1F684), "High-Speed Train");
        public static readonly Emoji bulletTrain = new Emoji(char.ConvertFromUtf32(0x1F685), "Bullet Train");
        public static readonly Emoji train = new Emoji(char.ConvertFromUtf32(0x1F686), "Train");
        public static readonly Emoji metro = new Emoji(char.ConvertFromUtf32(0x1F687), "Metro");
        public static readonly Emoji lightRail = new Emoji(char.ConvertFromUtf32(0x1F688), "Light Rail");
        public static readonly Emoji station = new Emoji(char.ConvertFromUtf32(0x1F689), "Station");
        public static readonly Emoji tram = new Emoji(char.ConvertFromUtf32(0x1F68A), "Tram");
        public static readonly Emoji tramCar = new Emoji(char.ConvertFromUtf32(0x1F68B), "Tram Car");
        public static readonly Emoji bus = new Emoji(char.ConvertFromUtf32(0x1F68C), "Bus");
        public static readonly Emoji oncomingBus = new Emoji(char.ConvertFromUtf32(0x1F68D), "Oncoming Bus");
        public static readonly Emoji trolleyBus = new Emoji(char.ConvertFromUtf32(0x1F68E), "Trolleybus");
        public static readonly Emoji busStop = new Emoji(char.ConvertFromUtf32(0x1F68F), "Bus Stop");
        public static readonly Emoji miniBus = new Emoji(char.ConvertFromUtf32(0x1F690), "Minibus");
        public static readonly Emoji ambulance = new Emoji(char.ConvertFromUtf32(0x1F691), "Ambulance");
        public static readonly Emoji policeCar = new Emoji(char.ConvertFromUtf32(0x1F693), "Police Car");
        public static readonly Emoji oncomingPoliceCar = new Emoji(char.ConvertFromUtf32(0x1F694), "Oncoming Police Car");
        public static readonly Emoji taxi = new Emoji(char.ConvertFromUtf32(0x1F695), "Taxi");
        public static readonly Emoji oncomingTaxi = new Emoji(char.ConvertFromUtf32(0x1F696), "Oncoming Taxi");
        public static readonly Emoji automobile = new Emoji(char.ConvertFromUtf32(0x1F697), "Automobile");
        public static readonly Emoji oncomingAutomobile = new Emoji(char.ConvertFromUtf32(0x1F698), "Oncoming Automobile");
        public static readonly Emoji sportUtilityVehicle = new Emoji(char.ConvertFromUtf32(0x1F699), "Sport Utility Vehicle");
        public static readonly Emoji deliveryTruck = new Emoji(char.ConvertFromUtf32(0x1F69A), "Delivery Truck");
        public static readonly Emoji articulatedLorry = new Emoji(char.ConvertFromUtf32(0x1F69B), "Articulated Lorry");
        public static readonly Emoji tractor = new Emoji(char.ConvertFromUtf32(0x1F69C), "Tractor");
        public static readonly Emoji monorail = new Emoji(char.ConvertFromUtf32(0x1F69D), "Monorail");
        public static readonly Emoji mountainRailway = new Emoji(char.ConvertFromUtf32(0x1F69E), "Mountain Railway");
        public static readonly Emoji suspensionRailway = new Emoji(char.ConvertFromUtf32(0x1F69F), "Suspension Railway");
        public static readonly Emoji mountainCableway = new Emoji(char.ConvertFromUtf32(0x1F6A0), "Mountain Cableway");
        public static readonly Emoji aerialTramway = new Emoji(char.ConvertFromUtf32(0x1F6A1), "Aerial Tramway");
        public static readonly Emoji ship = new Emoji(char.ConvertFromUtf32(0x1F6A2), "Ship");
        public static readonly Emoji speedBoat = new Emoji(char.ConvertFromUtf32(0x1F6A4), "Speedboat");
        public static readonly Emoji horizontalTrafficLight = new Emoji(char.ConvertFromUtf32(0x1F6A5), "Horizontal Traffic Light");
        public static readonly Emoji verticalTrafficLight = new Emoji(char.ConvertFromUtf32(0x1F6A6), "Vertical Traffic Light");
        public static readonly Emoji construction = new Emoji(char.ConvertFromUtf32(0x1F6A7), "Construction");
        public static readonly Emoji policeCarLight = new Emoji(char.ConvertFromUtf32(0x1F6A8), "Police Car Light");
        public static readonly Emoji bicycle = new Emoji(char.ConvertFromUtf32(0x1F6B2), "Bicycle");
        public static readonly Emoji stopSign = new Emoji(char.ConvertFromUtf32(0x1F6D1), "Stop Sign");
        public static readonly Emoji oilDrum = new Emoji(char.ConvertFromUtf32(0x1F6E2) + emojiStyle.Value, "Oil Drum");
        public static readonly Emoji motorway = new Emoji(char.ConvertFromUtf32(0x1F6E3) + emojiStyle.Value, "Motorway");
        public static readonly Emoji railwayTrack = new Emoji(char.ConvertFromUtf32(0x1F6E4) + emojiStyle.Value, "Railway Track");
        public static readonly Emoji motorBoat = new Emoji(char.ConvertFromUtf32(0x1F6E5) + emojiStyle.Value, "Motor Boat");
        public static readonly Emoji smallAirplane = new Emoji(char.ConvertFromUtf32(0x1F6E9) + emojiStyle.Value, "Small Airplane");
        public static readonly Emoji airplaneDeparture = new Emoji(char.ConvertFromUtf32(0x1F6EB), "Airplane Departure");
        public static readonly Emoji airplaneArrival = new Emoji(char.ConvertFromUtf32(0x1F6EC), "Airplane Arrival");
        public static readonly Emoji satellite = new Emoji(char.ConvertFromUtf32(0x1F6F0) + emojiStyle.Value, "Satellite");
        public static readonly Emoji passengerShip = new Emoji(char.ConvertFromUtf32(0x1F6F3) + emojiStyle.Value, "Passenger Ship");
        public static readonly Emoji kickScooter = new Emoji(char.ConvertFromUtf32(0x1F6F4), "Kick Scooter");
        public static readonly Emoji motorScooter = new Emoji(char.ConvertFromUtf32(0x1F6F5), "Motor Scooter");
        public static readonly Emoji canoe = new Emoji(char.ConvertFromUtf32(0x1F6F6), "Canoe");
        public static readonly Emoji flyingSaucer = new Emoji(char.ConvertFromUtf32(0x1F6F8), "Flying Saucer");
        public static readonly Emoji skateboard = new Emoji(char.ConvertFromUtf32(0x1F6F9), "Skateboard");
        public static readonly Emoji autoRickshaw = new Emoji(char.ConvertFromUtf32(0x1F6FA), "Auto Rickshaw");
        public static readonly Emoji pickupTruck = new Emoji(char.ConvertFromUtf32(0x1F6FB), "Pickup Truck");
        public static readonly Emoji rollerSkate = new Emoji(char.ConvertFromUtf32(0x1F6FC), "Roller Skate");
        public static readonly Emoji parachute = new Emoji(char.ConvertFromUtf32(0x1FA82), "Parachute");
        public static readonly Emoji anchor = new Emoji("\u2693", "Anchor");
        public static readonly Emoji ferry = new Emoji("\u26F4" + emojiStyle.Value, "Ferry");
        public static readonly Emoji sailboat = new Emoji("\u26F5", "Sailboat");
        public static readonly Emoji fuelPump = new Emoji("\u26FD", "Fuel Pump");
        public static readonly EmojiGroup vehicles = new EmojiGroup(
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
        private static readonly Emoji bloodTypeButtonA = new Emoji(char.ConvertFromUtf32(0x1F170), "A Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonB = new Emoji(char.ConvertFromUtf32(0x1F171), "B Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonO = new Emoji(char.ConvertFromUtf32(0x1F17E), "O Button (Blood Type)");
        private static readonly Emoji bloodTypeButtonAB = new Emoji(char.ConvertFromUtf32(0x1F18E), "AB Button (Blood Type)");
        public static readonly EmojiGroup bloodTypes = new EmojiGroup(
            "Blood Types", "Blood types",
            bloodTypeButtonA,
            bloodTypeButtonB,
            bloodTypeButtonO,
            bloodTypeButtonAB);

        public static readonly Emoji regionalIndicatorSymbolLetterA = new Emoji(char.ConvertFromUtf32(0x1F1E6), "Regional Indicator Symbol Letter A");
        public static readonly Emoji regionalIndicatorSymbolLetterB = new Emoji(char.ConvertFromUtf32(0x1F1E7), "Regional Indicator Symbol Letter B");
        public static readonly Emoji regionalIndicatorSymbolLetterC = new Emoji(char.ConvertFromUtf32(0x1F1E8), "Regional Indicator Symbol Letter C");
        public static readonly Emoji regionalIndicatorSymbolLetterD = new Emoji(char.ConvertFromUtf32(0x1F1E9), "Regional Indicator Symbol Letter D");
        public static readonly Emoji regionalIndicatorSymbolLetterE = new Emoji(char.ConvertFromUtf32(0x1F1EA), "Regional Indicator Symbol Letter E");
        public static readonly Emoji regionalIndicatorSymbolLetterF = new Emoji(char.ConvertFromUtf32(0x1F1EB), "Regional Indicator Symbol Letter F");
        public static readonly Emoji regionalIndicatorSymbolLetterG = new Emoji(char.ConvertFromUtf32(0x1F1EC), "Regional Indicator Symbol Letter G");
        public static readonly Emoji regionalIndicatorSymbolLetterH = new Emoji(char.ConvertFromUtf32(0x1F1ED), "Regional Indicator Symbol Letter H");
        public static readonly Emoji regionalIndicatorSymbolLetterI = new Emoji(char.ConvertFromUtf32(0x1F1EE), "Regional Indicator Symbol Letter I");
        public static readonly Emoji regionalIndicatorSymbolLetterJ = new Emoji(char.ConvertFromUtf32(0x1F1EF), "Regional Indicator Symbol Letter J");
        public static readonly Emoji regionalIndicatorSymbolLetterK = new Emoji(char.ConvertFromUtf32(0x1F1F0), "Regional Indicator Symbol Letter K");
        public static readonly Emoji regionalIndicatorSymbolLetterL = new Emoji(char.ConvertFromUtf32(0x1F1F1), "Regional Indicator Symbol Letter L");
        public static readonly Emoji regionalIndicatorSymbolLetterM = new Emoji(char.ConvertFromUtf32(0x1F1F2), "Regional Indicator Symbol Letter M");
        public static readonly Emoji regionalIndicatorSymbolLetterN = new Emoji(char.ConvertFromUtf32(0x1F1F3), "Regional Indicator Symbol Letter N");
        public static readonly Emoji regionalIndicatorSymbolLetterO = new Emoji(char.ConvertFromUtf32(0x1F1F4), "Regional Indicator Symbol Letter O");
        public static readonly Emoji regionalIndicatorSymbolLetterP = new Emoji(char.ConvertFromUtf32(0x1F1F5), "Regional Indicator Symbol Letter P");
        public static readonly Emoji regionalIndicatorSymbolLetterQ = new Emoji(char.ConvertFromUtf32(0x1F1F6), "Regional Indicator Symbol Letter Q");
        public static readonly Emoji regionalIndicatorSymbolLetterR = new Emoji(char.ConvertFromUtf32(0x1F1F7), "Regional Indicator Symbol Letter R");
        public static readonly Emoji regionalIndicatorSymbolLetterS = new Emoji(char.ConvertFromUtf32(0x1F1F8), "Regional Indicator Symbol Letter S");
        public static readonly Emoji regionalIndicatorSymbolLetterT = new Emoji(char.ConvertFromUtf32(0x1F1F9), "Regional Indicator Symbol Letter T");
        public static readonly Emoji regionalIndicatorSymbolLetterU = new Emoji(char.ConvertFromUtf32(0x1F1FA), "Regional Indicator Symbol Letter U");
        public static readonly Emoji regionalIndicatorSymbolLetterV = new Emoji(char.ConvertFromUtf32(0x1F1FB), "Regional Indicator Symbol Letter V");
        public static readonly Emoji regionalIndicatorSymbolLetterW = new Emoji(char.ConvertFromUtf32(0x1F1FC), "Regional Indicator Symbol Letter W");
        public static readonly Emoji regionalIndicatorSymbolLetterX = new Emoji(char.ConvertFromUtf32(0x1F1FD), "Regional Indicator Symbol Letter X");
        public static readonly Emoji regionalIndicatorSymbolLetterY = new Emoji(char.ConvertFromUtf32(0x1F1FE), "Regional Indicator Symbol Letter Y");
        public static readonly Emoji regionalIndicatorSymbolLetterZ = new Emoji(char.ConvertFromUtf32(0x1F1FF), "Regional Indicator Symbol Letter Z");
        public static readonly EmojiGroup regionIndicators = new EmojiGroup(
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

        public static readonly EmojiGroup japanese = new EmojiGroup(
            "Japanese", "Japanse symbology",
            new Emoji(char.ConvertFromUtf32(0x1F530), "Japanese Symbol for Beginner"),
            new Emoji(char.ConvertFromUtf32(0x1F201), "Japanese “Here” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F202) + emojiStyle.Value, "Japanese “Service Charge” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F21A), "Japanese “Free of Charge” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F22F), "Japanese “Reserved” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F232), "Japanese “Prohibited” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F233), "Japanese “Vacancy” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F234), "Japanese “Passing Grade” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F235), "Japanese “No Vacancy” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F236), "Japanese “Not Free of Charge” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F237) + emojiStyle.Value, "Japanese “Monthly Amount” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F238), "Japanese “Application” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F239), "Japanese “Discount” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F23A), "Japanese “Open for Business” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F250), "Japanese “Bargain” Button"),
            new Emoji(char.ConvertFromUtf32(0x1F251), "Japanese “Acceptable” Button"),
            new Emoji("\u3297" + emojiStyle.Value, "Japanese “Congratulations” Button"),
            new Emoji("\u3299" + emojiStyle.Value, "Japanese “Secret” Button"));

        public static readonly EmojiGroup clocks = new EmojiGroup(
            "Clocks", "Time-keeping pieces",
            new Emoji(char.ConvertFromUtf32(0x1F550), "One O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F551), "Two O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F552), "Three O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F553), "Four O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F554), "Five O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F555), "Six O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F556), "Seven O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F557), "Eight O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F558), "Nine O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F559), "Ten O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F55A), "Eleven O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F55B), "Twelve O’Clock"),
            new Emoji(char.ConvertFromUtf32(0x1F55C), "One-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F55D), "Two-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F55E), "Three-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F55F), "Four-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F560), "Five-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F561), "Six-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F562), "Seven-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F563), "Eight-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F564), "Nine-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F565), "Ten-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F566), "Eleven-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F567), "Twelve-Thirty"),
            new Emoji(char.ConvertFromUtf32(0x1F570) + emojiStyle.Value, "Mantelpiece Clock"),
            new Emoji("\u231A", "Watch"),
            new Emoji("\u23F0", "Alarm Clock"),
            new Emoji("\u23F1" + emojiStyle.Value, "Stopwatch"),
            new Emoji("\u23F2" + emojiStyle.Value, "Timer Clock"),
            new Emoji("\u231B", "Hourglass Done"),
            new Emoji("\u23F3", "Hourglass Not Done"));

        public static readonly Emoji clockwiseVerticalArrows = new Emoji(char.ConvertFromUtf32(0x1F503) + emojiStyle.Value, "Clockwise Vertical Arrows");
        public static readonly Emoji counterclockwiseArrowsButton = new Emoji(char.ConvertFromUtf32(0x1F504) + emojiStyle.Value, "Counterclockwise Arrows Button");
        public static readonly Emoji leftRightArrow = new Emoji("\u2194" + emojiStyle.Value, "Left-Right Arrow");
        public static readonly Emoji upDownArrow = new Emoji("\u2195" + emojiStyle.Value, "Up-Down Arrow");
        public static readonly Emoji upLeftArrow = new Emoji("\u2196" + emojiStyle.Value, "Up-Left Arrow");
        public static readonly Emoji upRightArrow = new Emoji("\u2197" + emojiStyle.Value, "Up-Right Arrow");
        public static readonly Emoji downRightArrow = new Emoji("\u2198", "Down-Right Arrow");
        public static readonly Emoji downRightArrowText = new Emoji("\u2198" + textStyle.Value, "Down-Right Arrow");
        public static readonly Emoji downRightArrowEmoji = new Emoji("\u2198" + emojiStyle.Value, "Down-Right Arrow");
        public static readonly Emoji downLeftArrow = new Emoji("\u2199" + emojiStyle.Value, "Down-Left Arrow");
        public static readonly Emoji rightArrowCurvingLeft = new Emoji("\u21A9" + emojiStyle.Value, "Right Arrow Curving Left");
        public static readonly Emoji leftArrowCurvingRight = new Emoji("\u21AA" + emojiStyle.Value, "Left Arrow Curving Right");
        public static readonly Emoji rightArrow = new Emoji("\u27A1" + emojiStyle.Value, "Right Arrow");
        public static readonly Emoji rightArrowCurvingUp = new Emoji("\u2934" + emojiStyle.Value, "Right Arrow Curving Up");
        public static readonly Emoji rightArrowCurvingDown = new Emoji("\u2935" + emojiStyle.Value, "Right Arrow Curving Down");
        public static readonly Emoji leftArrow = new Emoji("\u2B05" + emojiStyle.Value, "Left Arrow");
        public static readonly Emoji upArrow = new Emoji("\u2B06" + emojiStyle.Value, "Up Arrow");
        public static readonly Emoji downArrow = new Emoji("\u2B07" + emojiStyle.Value, "Down Arrow");
        public static readonly EmojiGroup arrows = new EmojiGroup(
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

        public static readonly Emoji redCircle = new Emoji(char.ConvertFromUtf32(0x1F534), "Red Circle");
        public static readonly Emoji blueCircle = new Emoji(char.ConvertFromUtf32(0x1F535), "Blue Circle");
        public static readonly Emoji largeOrangeDiamond = new Emoji(char.ConvertFromUtf32(0x1F536), "Large Orange Diamond");
        public static readonly Emoji largeBlueDiamond = new Emoji(char.ConvertFromUtf32(0x1F537), "Large Blue Diamond");
        public static readonly Emoji smallOrangeDiamond = new Emoji(char.ConvertFromUtf32(0x1F538), "Small Orange Diamond");
        public static readonly Emoji smallBlueDiamond = new Emoji(char.ConvertFromUtf32(0x1F539), "Small Blue Diamond");
        public static readonly Emoji redTrianglePointedUp = new Emoji(char.ConvertFromUtf32(0x1F53A), "Red Triangle Pointed Up");
        public static readonly Emoji redTrianglePointedDown = new Emoji(char.ConvertFromUtf32(0x1F53B), "Red Triangle Pointed Down");
        public static readonly Emoji orangeCircle = new Emoji(char.ConvertFromUtf32(0x1F7E0), "Orange Circle");
        public static readonly Emoji yellowCircle = new Emoji(char.ConvertFromUtf32(0x1F7E1), "Yellow Circle");
        public static readonly Emoji greenCircle = new Emoji(char.ConvertFromUtf32(0x1F7E2), "Green Circle");
        public static readonly Emoji purpleCircle = new Emoji(char.ConvertFromUtf32(0x1F7E3), "Purple Circle");
        public static readonly Emoji brownCircle = new Emoji(char.ConvertFromUtf32(0x1F7E4), "Brown Circle");
        public static readonly Emoji hollowRedCircle = new Emoji("\u2B55", "Hollow Red Circle");
        public static readonly Emoji whiteCircle = new Emoji("\u26AA", "White Circle");
        public static readonly Emoji blackCircle = new Emoji("\u26AB", "Black Circle");
        public static readonly Emoji redSquare = new Emoji(char.ConvertFromUtf32(0x1F7E5), "Red Square");
        public static readonly Emoji blueSquare = new Emoji(char.ConvertFromUtf32(0x1F7E6), "Blue Square");
        public static readonly Emoji orangeSquare = new Emoji(char.ConvertFromUtf32(0x1F7E7), "Orange Square");
        public static readonly Emoji yellowSquare = new Emoji(char.ConvertFromUtf32(0x1F7E8), "Yellow Square");
        public static readonly Emoji greenSquare = new Emoji(char.ConvertFromUtf32(0x1F7E9), "Green Square");
        public static readonly Emoji purpleSquare = new Emoji(char.ConvertFromUtf32(0x1F7EA), "Purple Square");
        public static readonly Emoji brownSquare = new Emoji(char.ConvertFromUtf32(0x1F7EB), "Brown Square");
        public static readonly Emoji blackSquareButton = new Emoji(char.ConvertFromUtf32(0x1F532), "Black Square Button");
        public static readonly Emoji whiteSquareButton = new Emoji(char.ConvertFromUtf32(0x1F533), "White Square Button");
        public static readonly Emoji blackSmallSquare = new Emoji("\u25AA" + emojiStyle.Value, "Black Small Square");
        public static readonly Emoji whiteSmallSquare = new Emoji("\u25AB" + emojiStyle.Value, "White Small Square");
        public static readonly Emoji whiteMediumSmallSquare = new Emoji("\u25FD", "White Medium-Small Square");
        public static readonly Emoji blackMediumSmallSquare = new Emoji("\u25FE", "Black Medium-Small Square");
        public static readonly Emoji whiteMediumSquare = new Emoji("\u25FB" + emojiStyle.Value, "White Medium Square");
        public static readonly Emoji blackMediumSquare = new Emoji("\u25FC" + emojiStyle.Value, "Black Medium Square");
        public static readonly Emoji blackLargeSquare = new Emoji("\u2B1B", "Black Large Square");
        public static readonly Emoji whiteLargeSquare = new Emoji("\u2B1C", "White Large Square");
        public static readonly Emoji star = new Emoji("\u2B50", "Star");
        public static readonly Emoji diamondWithADot = new Emoji(char.ConvertFromUtf32(0x1F4A0), "Diamond with a Dot");
        public static readonly EmojiGroup shapes = new EmojiGroup(
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

        public static readonly Emoji clearButton = new Emoji(char.ConvertFromUtf32(0x1F191), "CL Button");
        public static readonly Emoji coolButton = new Emoji(char.ConvertFromUtf32(0x1F192), "Cool Button");
        public static readonly Emoji freeButton = new Emoji(char.ConvertFromUtf32(0x1F193), "Free Button");
        public static readonly Emoji idButton = new Emoji(char.ConvertFromUtf32(0x1F194), "ID Button");
        public static readonly Emoji newButton = new Emoji(char.ConvertFromUtf32(0x1F195), "New Button");
        public static readonly Emoji ngButton = new Emoji(char.ConvertFromUtf32(0x1F196), "NG Button");
        public static readonly Emoji okButton = new Emoji(char.ConvertFromUtf32(0x1F197), "OK Button");
        public static readonly Emoji sosButton = new Emoji(char.ConvertFromUtf32(0x1F198), "SOS Button");
        public static readonly Emoji upButton = new Emoji(char.ConvertFromUtf32(0x1F199), "Up! Button");
        public static readonly Emoji vsButton = new Emoji(char.ConvertFromUtf32(0x1F19A), "Vs Button");
        public static readonly Emoji radioButton = new Emoji(char.ConvertFromUtf32(0x1F518), "Radio Button");
        public static readonly Emoji backArrow = new Emoji(char.ConvertFromUtf32(0x1F519), "Back Arrow");
        public static readonly Emoji endArrow = new Emoji(char.ConvertFromUtf32(0x1F51A), "End Arrow");
        public static readonly Emoji onArrow = new Emoji(char.ConvertFromUtf32(0x1F51B), "On! Arrow");
        public static readonly Emoji soonArrow = new Emoji(char.ConvertFromUtf32(0x1F51C), "Soon Arrow");
        public static readonly Emoji topArrow = new Emoji(char.ConvertFromUtf32(0x1F51D), "Top Arrow");
        public static readonly Emoji checkBoxWithCheck = new Emoji("\u2611" + emojiStyle.Value, "Check Box with Check");
        public static readonly Emoji inputLatinUppercase = new Emoji(char.ConvertFromUtf32(0x1F520), "Input Latin Uppercase");
        public static readonly Emoji inputLatinLowercase = new Emoji(char.ConvertFromUtf32(0x1F521), "Input Latin Lowercase");
        public static readonly Emoji inputNumbers = new Emoji(char.ConvertFromUtf32(0x1F522), "Input Numbers");
        public static readonly Emoji inputSymbols = new Emoji(char.ConvertFromUtf32(0x1F523), "Input Symbols");
        public static readonly Emoji inputLatinLetters = new Emoji(char.ConvertFromUtf32(0x1F524), "Input Latin Letters");
        public static readonly Emoji shuffleTracksButton = new Emoji(char.ConvertFromUtf32(0x1F500), "Shuffle Tracks Button");
        public static readonly Emoji repeatButton = new Emoji(char.ConvertFromUtf32(0x1F501), "Repeat Button");
        public static readonly Emoji repeatSingleButton = new Emoji(char.ConvertFromUtf32(0x1F502), "Repeat Single Button");
        public static readonly Emoji upwardsButton = new Emoji(char.ConvertFromUtf32(0x1F53C), "Upwards Button");
        public static readonly Emoji downwardsButton = new Emoji(char.ConvertFromUtf32(0x1F53D), "Downwards Button");
        public static readonly Emoji playButton = new Emoji("\u25B6" + emojiStyle.Value, "Play Button");
        public static readonly Emoji reverseButton = new Emoji("\u25C0" + emojiStyle.Value, "Reverse Button");
        public static readonly Emoji ejectButton = new Emoji("\u23CF" + emojiStyle.Value, "Eject Button");
        public static readonly Emoji fastForwardButton = new Emoji("\u23E9", "Fast-Forward Button");
        public static readonly Emoji fastReverseButton = new Emoji("\u23EA", "Fast Reverse Button");
        public static readonly Emoji fastUpButton = new Emoji("\u23EB", "Fast Up Button");
        public static readonly Emoji fastDownButton = new Emoji("\u23EC", "Fast Down Button");
        public static readonly Emoji nextTrackButton = new Emoji("\u23ED" + emojiStyle.Value, "Next Track Button");
        public static readonly Emoji lastTrackButton = new Emoji("\u23EE" + emojiStyle.Value, "Last Track Button");
        public static readonly Emoji playOrPauseButton = new Emoji("\u23EF" + emojiStyle.Value, "Play or Pause Button");
        public static readonly Emoji pauseButton = new Emoji("\u23F8" + emojiStyle.Value, "Pause Button");
        public static readonly Emoji stopButton = new Emoji("\u23F9" + emojiStyle.Value, "Stop Button");
        public static readonly Emoji recordButton = new Emoji("\u23FA" + emojiStyle.Value, "Record Button");
        public static readonly EmojiGroup buttons = new EmojiGroup(
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

        public static readonly EmojiGroup zodiac = new EmojiGroup(
            "Zodiac", "The symbology of astrology",
            new Emoji("\u2648", "Aries"),
            new Emoji("\u2649", "Taurus"),
            new Emoji("\u264A", "Gemini"),
            new Emoji("\u264B", "Cancer"),
            new Emoji("\u264C", "Leo"),
            new Emoji("\u264D", "Virgo"),
            new Emoji("\u264E", "Libra"),
            new Emoji("\u264F", "Scorpio"),
            new Emoji("\u2650", "Sagittarius"),
            new Emoji("\u2651", "Capricorn"),
            new Emoji("\u2652", "Aquarius"),
            new Emoji("\u2653", "Pisces"),
            new Emoji("\u26CE", "Ophiuchus"));

        public static readonly EmojiGroup numbers = new EmojiGroup(
            "Numbers", "Numbers",
            new Emoji("0" + emojiStyle.Value, "Digit Zero"),
            new Emoji("1" + emojiStyle.Value, "Digit One"),
            new Emoji("2" + emojiStyle.Value, "Digit Two"),
            new Emoji("3" + emojiStyle.Value, "Digit Three"),
            new Emoji("4" + emojiStyle.Value, "Digit Four"),
            new Emoji("5" + emojiStyle.Value, "Digit Five"),
            new Emoji("6" + emojiStyle.Value, "Digit Six"),
            new Emoji("7" + emojiStyle.Value, "Digit Seven"),
            new Emoji("8" + emojiStyle.Value, "Digit Eight"),
            new Emoji("9" + emojiStyle.Value, "Digit Nine"),
            new Emoji("*" + emojiStyle.Value, "Asterisk"),
            new Emoji("#" + emojiStyle.Value, "Number Sign"),
            new Emoji("0" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Zero"),
            new Emoji("1" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit One"),
            new Emoji("2" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Two"),
            new Emoji("3" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Three"),
            new Emoji("4" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Four"),
            new Emoji("5" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Five"),
            new Emoji("6" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Six"),
            new Emoji("7" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Seven"),
            new Emoji("8" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Eight"),
            new Emoji("9" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Digit Nine"),
            new Emoji("*" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Asterisk"),
            new Emoji("#" + emojiStyle.Value + combiningEnclosingKeycap.Value, "Keycap Number Sign"),
            new Emoji(char.ConvertFromUtf32(0x1F51F), "Keycap: 10"));

        public static readonly Emoji tagPlusSign = new Emoji(char.ConvertFromUtf32(0xE002B), "Tag Plus Sign");
        public static readonly Emoji tagMinusHyphen = new Emoji(char.ConvertFromUtf32(0xE002D), "Tag Hyphen-Minus");
        public static readonly Emoji tagLatinSmallLetterA = new Emoji(char.ConvertFromUtf32(0xE0061), "Tag Latin Small Letter A");
        public static readonly Emoji tagLatinSmallLetterB = new Emoji(char.ConvertFromUtf32(0xE0062), "Tag Latin Small Letter B");
        public static readonly Emoji tagLatinSmallLetterC = new Emoji(char.ConvertFromUtf32(0xE0063), "Tag Latin Small Letter C");
        public static readonly Emoji tagLatinSmallLetterD = new Emoji(char.ConvertFromUtf32(0xE0064), "Tag Latin Small Letter D");
        public static readonly Emoji tagLatinSmallLetterE = new Emoji(char.ConvertFromUtf32(0xE0065), "Tag Latin Small Letter E");
        public static readonly Emoji tagLatinSmallLetterF = new Emoji(char.ConvertFromUtf32(0xE0066), "Tag Latin Small Letter F");
        public static readonly Emoji tagLatinSmallLetterG = new Emoji(char.ConvertFromUtf32(0xE0067), "Tag Latin Small Letter G");
        public static readonly Emoji tagLatinSmallLetterH = new Emoji(char.ConvertFromUtf32(0xE0068), "Tag Latin Small Letter H");
        public static readonly Emoji tagLatinSmallLetterI = new Emoji(char.ConvertFromUtf32(0xE0069), "Tag Latin Small Letter I");
        public static readonly Emoji tagLatinSmallLetterJ = new Emoji(char.ConvertFromUtf32(0xE006A), "Tag Latin Small Letter J");
        public static readonly Emoji tagLatinSmallLetterK = new Emoji(char.ConvertFromUtf32(0xE006B), "Tag Latin Small Letter K");
        public static readonly Emoji tagLatinSmallLetterL = new Emoji(char.ConvertFromUtf32(0xE006C), "Tag Latin Small Letter L");
        public static readonly Emoji tagLatinSmallLetterM = new Emoji(char.ConvertFromUtf32(0xE006D), "Tag Latin Small Letter M");
        public static readonly Emoji tagLatinSmallLetterN = new Emoji(char.ConvertFromUtf32(0xE006E), "Tag Latin Small Letter N");
        public static readonly Emoji tagLatinSmallLetterO = new Emoji(char.ConvertFromUtf32(0xE006F), "Tag Latin Small Letter O");
        public static readonly Emoji tagLatinSmallLetterP = new Emoji(char.ConvertFromUtf32(0xE0070), "Tag Latin Small Letter P");
        public static readonly Emoji tagLatinSmallLetterQ = new Emoji(char.ConvertFromUtf32(0xE0071), "Tag Latin Small Letter Q");
        public static readonly Emoji tagLatinSmallLetterR = new Emoji(char.ConvertFromUtf32(0xE0072), "Tag Latin Small Letter R");
        public static readonly Emoji tagLatinSmallLetterS = new Emoji(char.ConvertFromUtf32(0xE0073), "Tag Latin Small Letter S");
        public static readonly Emoji tagLatinSmallLetterT = new Emoji(char.ConvertFromUtf32(0xE0074), "Tag Latin Small Letter T");
        public static readonly Emoji tagLatinSmallLetterU = new Emoji(char.ConvertFromUtf32(0xE0075), "Tag Latin Small Letter U");
        public static readonly Emoji tagLatinSmallLetterV = new Emoji(char.ConvertFromUtf32(0xE0076), "Tag Latin Small Letter V");
        public static readonly Emoji tagLatinSmallLetterW = new Emoji(char.ConvertFromUtf32(0xE0077), "Tag Latin Small Letter W");
        public static readonly Emoji tagLatinSmallLetterX = new Emoji(char.ConvertFromUtf32(0xE0078), "Tag Latin Small Letter X");
        public static readonly Emoji tagLatinSmallLetterY = new Emoji(char.ConvertFromUtf32(0xE0079), "Tag Latin Small Letter Y");
        public static readonly Emoji tagLatinSmallLetterZ = new Emoji(char.ConvertFromUtf32(0xE007A), "Tag Latin Small Letter Z");
        public static readonly Emoji cancelTag = new Emoji(char.ConvertFromUtf32(0xE007F), "Cancel Tag");
        public static readonly EmojiGroup tags = new EmojiGroup(
            "Tags", "Tags",
            new Emoji(char.ConvertFromUtf32(0xE0020), "Tag Space"),
            new Emoji(char.ConvertFromUtf32(0xE0021), "Tag Exclamation Mark"),
            new Emoji(char.ConvertFromUtf32(0xE0022), "Tag Quotation Mark"),
            new Emoji(char.ConvertFromUtf32(0xE0023), "Tag Number Sign"),
            new Emoji(char.ConvertFromUtf32(0xE0024), "Tag Dollar Sign"),
            new Emoji(char.ConvertFromUtf32(0xE0025), "Tag Percent Sign"),
            new Emoji(char.ConvertFromUtf32(0xE0026), "Tag Ampersand"),
            new Emoji(char.ConvertFromUtf32(0xE0027), "Tag Apostrophe"),
            new Emoji(char.ConvertFromUtf32(0xE0028), "Tag Left Parenthesis"),
            new Emoji(char.ConvertFromUtf32(0xE0029), "Tag Right Parenthesis"),
            new Emoji(char.ConvertFromUtf32(0xE002A), "Tag Asterisk"),
            tagPlusSign,
            new Emoji(char.ConvertFromUtf32(0xE002C), "Tag Comma"),
            tagMinusHyphen,
            new Emoji(char.ConvertFromUtf32(0xE002E), "Tag Full Stop"),
            new Emoji(char.ConvertFromUtf32(0xE002F), "Tag Solidus"),
            new Emoji(char.ConvertFromUtf32(0xE0030), "Tag Digit Zero"),
            new Emoji(char.ConvertFromUtf32(0xE0031), "Tag Digit One"),
            new Emoji(char.ConvertFromUtf32(0xE0032), "Tag Digit Two"),
            new Emoji(char.ConvertFromUtf32(0xE0033), "Tag Digit Three"),
            new Emoji(char.ConvertFromUtf32(0xE0034), "Tag Digit Four"),
            new Emoji(char.ConvertFromUtf32(0xE0035), "Tag Digit Five"),
            new Emoji(char.ConvertFromUtf32(0xE0036), "Tag Digit Six"),
            new Emoji(char.ConvertFromUtf32(0xE0037), "Tag Digit Seven"),
            new Emoji(char.ConvertFromUtf32(0xE0038), "Tag Digit Eight"),
            new Emoji(char.ConvertFromUtf32(0xE0039), "Tag Digit Nine"),
            new Emoji(char.ConvertFromUtf32(0xE003A), "Tag Colon"),
            new Emoji(char.ConvertFromUtf32(0xE003B), "Tag Semicolon"),
            new Emoji(char.ConvertFromUtf32(0xE003C), "Tag Less-Than Sign"),
            new Emoji(char.ConvertFromUtf32(0xE003D), "Tag Equals Sign"),
            new Emoji(char.ConvertFromUtf32(0xE003E), "Tag Greater-Than Sign"),
            new Emoji(char.ConvertFromUtf32(0xE003F), "Tag Question Mark"),
            new Emoji(char.ConvertFromUtf32(0xE0040), "Tag Commercial at"),
            new Emoji(char.ConvertFromUtf32(0xE0041), "Tag Latin Capital Letter a"),
            new Emoji(char.ConvertFromUtf32(0xE0042), "Tag Latin Capital Letter B"),
            new Emoji(char.ConvertFromUtf32(0xE0043), "Tag Latin Capital Letter C"),
            new Emoji(char.ConvertFromUtf32(0xE0044), "Tag Latin Capital Letter D"),
            new Emoji(char.ConvertFromUtf32(0xE0045), "Tag Latin Capital Letter E"),
            new Emoji(char.ConvertFromUtf32(0xE0046), "Tag Latin Capital Letter F"),
            new Emoji(char.ConvertFromUtf32(0xE0047), "Tag Latin Capital Letter G"),
            new Emoji(char.ConvertFromUtf32(0xE0048), "Tag Latin Capital Letter H"),
            new Emoji(char.ConvertFromUtf32(0xE0049), "Tag Latin Capital Letter I"),
            new Emoji(char.ConvertFromUtf32(0xE004A), "Tag Latin Capital Letter J"),
            new Emoji(char.ConvertFromUtf32(0xE004B), "Tag Latin Capital Letter K"),
            new Emoji(char.ConvertFromUtf32(0xE004C), "Tag Latin Capital Letter L"),
            new Emoji(char.ConvertFromUtf32(0xE004D), "Tag Latin Capital Letter M"),
            new Emoji(char.ConvertFromUtf32(0xE004E), "Tag Latin Capital Letter N"),
            new Emoji(char.ConvertFromUtf32(0xE004F), "Tag Latin Capital Letter O"),
            new Emoji(char.ConvertFromUtf32(0xE0050), "Tag Latin Capital Letter P"),
            new Emoji(char.ConvertFromUtf32(0xE0051), "Tag Latin Capital Letter Q"),
            new Emoji(char.ConvertFromUtf32(0xE0052), "Tag Latin Capital Letter R"),
            new Emoji(char.ConvertFromUtf32(0xE0053), "Tag Latin Capital Letter S"),
            new Emoji(char.ConvertFromUtf32(0xE0054), "Tag Latin Capital Letter T"),
            new Emoji(char.ConvertFromUtf32(0xE0055), "Tag Latin Capital Letter U"),
            new Emoji(char.ConvertFromUtf32(0xE0056), "Tag Latin Capital Letter V"),
            new Emoji(char.ConvertFromUtf32(0xE0057), "Tag Latin Capital Letter W"),
            new Emoji(char.ConvertFromUtf32(0xE0058), "Tag Latin Capital Letter X"),
            new Emoji(char.ConvertFromUtf32(0xE0059), "Tag Latin Capital Letter Y"),
            new Emoji(char.ConvertFromUtf32(0xE005A), "Tag Latin Capital Letter Z"),
            new Emoji(char.ConvertFromUtf32(0xE005B), "Tag Left Square Bracket"),
            new Emoji(char.ConvertFromUtf32(0xE005C), "Tag Reverse Solidus"),
            new Emoji(char.ConvertFromUtf32(0xE005D), "Tag Right Square Bracket"),
            new Emoji(char.ConvertFromUtf32(0xE005E), "Tag Circumflex Accent"),
            new Emoji(char.ConvertFromUtf32(0xE005F), "Tag Low Line"),
            new Emoji(char.ConvertFromUtf32(0xE0060), "Tag Grave Accent"),
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
            new Emoji(char.ConvertFromUtf32(0xE007B), "Tag Left Curly Bracket"),
            new Emoji(char.ConvertFromUtf32(0xE007C), "Tag Vertical Line"),
            new Emoji(char.ConvertFromUtf32(0xE007D), "Tag Right Curly Bracket"),
            new Emoji(char.ConvertFromUtf32(0xE007E), "Tag Tilde"),
            cancelTag);

        public static readonly EmojiGroup math = new EmojiGroup(
            "Math", "Math",
            new Emoji("\u2716" + emojiStyle.Value, "Multiply"),
            new Emoji("\u2795", "Plus"),
            new Emoji("\u2796", "Minus"),
            new Emoji("\u2797", "Divide"));

        public static readonly EmojiGroup games = new EmojiGroup(
            "Games", "Games",
            new Emoji("\u2660" + emojiStyle.Value, "Spade Suit"),
            new Emoji("\u2663" + emojiStyle.Value, "Club Suit"),
            new Emoji("\u2665" + emojiStyle.Value, "Heart Suit"),
            new Emoji("\u2666" + emojiStyle.Value, "Diamond Suit"),
            new Emoji(char.ConvertFromUtf32(0x1F004), "Mahjong Red Dragon"),
            new Emoji(char.ConvertFromUtf32(0x1F0CF), "Joker"),
            new Emoji(char.ConvertFromUtf32(0x1F3AF), "Direct Hit"),
            new Emoji(char.ConvertFromUtf32(0x1F3B0), "Slot Machine"),
            new Emoji(char.ConvertFromUtf32(0x1F3B1), "Pool 8 Ball"),
            new Emoji(char.ConvertFromUtf32(0x1F3B2), "Game Die"),
            new Emoji(char.ConvertFromUtf32(0x1F3B3), "Bowling"),
            new Emoji(char.ConvertFromUtf32(0x1F3B4), "Flower Playing Cards"),
            new Emoji(char.ConvertFromUtf32(0x1F9E9), "Puzzle Piece"),
            new Emoji("\u265F" + emojiStyle.Value, "Chess Pawn"),
            new Emoji(char.ConvertFromUtf32(0x1FA80), "Yo-Yo"),
            new Emoji(char.ConvertFromUtf32(0x1FA83), "Boomerang"),
            new Emoji(char.ConvertFromUtf32(0x1FA86), "Nesting Dolls"),
            new Emoji(char.ConvertFromUtf32(0x1FA81), "Kite"));

        public static readonly EmojiGroup sportsEquipment = new EmojiGroup(
            "Sports Equipment", "Sports equipment",
            new Emoji(char.ConvertFromUtf32(0x1F3BD), "Running Shirt"),
            new Emoji(char.ConvertFromUtf32(0x1F3BE), "Tennis"),
            new Emoji(char.ConvertFromUtf32(0x1F3BF), "Skis"),
            new Emoji(char.ConvertFromUtf32(0x1F3C0), "Basketball"),
            new Emoji(char.ConvertFromUtf32(0x1F3C5), "Sports Medal"),
            new Emoji(char.ConvertFromUtf32(0x1F3C6), "Trophy"),
            new Emoji(char.ConvertFromUtf32(0x1F3C8), "American Football"),
            new Emoji(char.ConvertFromUtf32(0x1F3C9), "Rugby Football"),
            new Emoji(char.ConvertFromUtf32(0x1F3CF), "Cricket Game"),
            new Emoji(char.ConvertFromUtf32(0x1F3D0), "Volleyball"),
            new Emoji(char.ConvertFromUtf32(0x1F3D1), "Field Hockey"),
            new Emoji(char.ConvertFromUtf32(0x1F3D2), "Ice Hockey"),
            new Emoji(char.ConvertFromUtf32(0x1F3D3), "Ping Pong"),
            new Emoji(char.ConvertFromUtf32(0x1F3F8), "Badminton"),
            new Emoji(char.ConvertFromUtf32(0x1F6F7), "Sled"),
            new Emoji(char.ConvertFromUtf32(0x1F945), "Goal Net"),
            new Emoji(char.ConvertFromUtf32(0x1F947), "1st Place Medal"),
            new Emoji(char.ConvertFromUtf32(0x1F948), "2nd Place Medal"),
            new Emoji(char.ConvertFromUtf32(0x1F949), "3rd Place Medal"),
            new Emoji(char.ConvertFromUtf32(0x1F94A), "Boxing Glove"),
            new Emoji(char.ConvertFromUtf32(0x1F94C), "Curling Stone"),
            new Emoji(char.ConvertFromUtf32(0x1F94D), "Lacrosse"),
            new Emoji(char.ConvertFromUtf32(0x1F94E), "Softball"),
            new Emoji(char.ConvertFromUtf32(0x1F94F), "Flying Disc"),
            new Emoji("\u26BD", "Soccer Ball"),
            new Emoji("\u26BE", "Baseball"),
            new Emoji("\u26F8" + emojiStyle.Value, "Ice Skate"));

        public static readonly Emoji safetyVest = new Emoji(char.ConvertFromUtf32(0x1F9BA), "Safety Vest");

        public static readonly EmojiGroup clothing = new EmojiGroup(
            "Clothing", "Clothing",
            new Emoji(char.ConvertFromUtf32(0x1F3A9), "Top Hat"),
            new Emoji(char.ConvertFromUtf32(0x1F93F), "Diving Mask"),
            new Emoji(char.ConvertFromUtf32(0x1F452), "Woman’s Hat"),
            new Emoji(char.ConvertFromUtf32(0x1F453), "Glasses"),
            new Emoji(char.ConvertFromUtf32(0x1F576) + emojiStyle.Value, "Sunglasses"),
            new Emoji(char.ConvertFromUtf32(0x1F454), "Necktie"),
            new Emoji(char.ConvertFromUtf32(0x1F455), "T-Shirt"),
            new Emoji(char.ConvertFromUtf32(0x1F456), "Jeans"),
            new Emoji(char.ConvertFromUtf32(0x1F457), "Dress"),
            new Emoji(char.ConvertFromUtf32(0x1F458), "Kimono"),
            new Emoji(char.ConvertFromUtf32(0x1F459), "Bikini"),
            new Emoji(char.ConvertFromUtf32(0x1F45A), "Woman’s Clothes"),
            new Emoji(char.ConvertFromUtf32(0x1F45B), "Purse"),
            new Emoji(char.ConvertFromUtf32(0x1F45C), "Handbag"),
            new Emoji(char.ConvertFromUtf32(0x1F45D), "Clutch Bag"),
            new Emoji(char.ConvertFromUtf32(0x1F45E), "Man’s Shoe"),
            new Emoji(char.ConvertFromUtf32(0x1F45F), "Running Shoe"),
            new Emoji(char.ConvertFromUtf32(0x1F460), "High-Heeled Shoe"),
            new Emoji(char.ConvertFromUtf32(0x1F461), "Woman’s Sandal"),
            new Emoji(char.ConvertFromUtf32(0x1F462), "Woman’s Boot"),
            new Emoji(char.ConvertFromUtf32(0x1F94B), "Martial Arts Uniform"),
            new Emoji(char.ConvertFromUtf32(0x1F97B), "Sari"),
            new Emoji(char.ConvertFromUtf32(0x1F97C), "Lab Coat"),
            new Emoji(char.ConvertFromUtf32(0x1F97D), "Goggles"),
            new Emoji(char.ConvertFromUtf32(0x1F97E), "Hiking Boot"),
            new Emoji(char.ConvertFromUtf32(0x1F97F), "Flat Shoe"),
            whiteCane,
            safetyVest,
            new Emoji(char.ConvertFromUtf32(0x1F9E2), "Billed Cap"),
            new Emoji(char.ConvertFromUtf32(0x1F9E3), "Scarf"),
            new Emoji(char.ConvertFromUtf32(0x1F9E4), "Gloves"),
            new Emoji(char.ConvertFromUtf32(0x1F9E5), "Coat"),
            new Emoji(char.ConvertFromUtf32(0x1F9E6), "Socks"),
            new Emoji(char.ConvertFromUtf32(0x1F9FF), "Nazar Amulet"),
            new Emoji(char.ConvertFromUtf32(0x1FA70), "Ballet Shoes"),
            new Emoji(char.ConvertFromUtf32(0x1FA71), "One-Piece Swimsuit"),
            new Emoji(char.ConvertFromUtf32(0x1FA72), "Briefs"),
            new Emoji(char.ConvertFromUtf32(0x1FA73), "Shorts"));

        public static readonly EmojiGroup town = new EmojiGroup(
            "Town", "Town",
            new Emoji(char.ConvertFromUtf32(0x1F3D7) + emojiStyle.Value, "Building Construction"),
            new Emoji(char.ConvertFromUtf32(0x1F3D8) + emojiStyle.Value, "Houses"),
            new Emoji(char.ConvertFromUtf32(0x1F3D9) + emojiStyle.Value, "Cityscape"),
            new Emoji(char.ConvertFromUtf32(0x1F3DA) + emojiStyle.Value, "Derelict House"),
            new Emoji(char.ConvertFromUtf32(0x1F3DB) + emojiStyle.Value, "Classical Building"),
            new Emoji(char.ConvertFromUtf32(0x1F3DC) + emojiStyle.Value, "Desert"),
            new Emoji(char.ConvertFromUtf32(0x1F3DD) + emojiStyle.Value, "Desert Island"),
            new Emoji(char.ConvertFromUtf32(0x1F3DE) + emojiStyle.Value, "National Park"),
            new Emoji(char.ConvertFromUtf32(0x1F3DF) + emojiStyle.Value, "Stadium"),
            new Emoji(char.ConvertFromUtf32(0x1F3E0), "House"),
            new Emoji(char.ConvertFromUtf32(0x1F3E1), "House with Garden"),
            new Emoji(char.ConvertFromUtf32(0x1F3E2), "Office Building"),
            new Emoji(char.ConvertFromUtf32(0x1F3E3), "Japanese Post Office"),
            new Emoji(char.ConvertFromUtf32(0x1F3E4), "Post Office"),
            new Emoji(char.ConvertFromUtf32(0x1F3E5), "Hospital"),
            new Emoji(char.ConvertFromUtf32(0x1F3E6), "Bank"),
            new Emoji(char.ConvertFromUtf32(0x1F3E7), "ATM Sign"),
            new Emoji(char.ConvertFromUtf32(0x1F3E8), "Hotel"),
            new Emoji(char.ConvertFromUtf32(0x1F3E9), "Love Hotel"),
            new Emoji(char.ConvertFromUtf32(0x1F3EA), "Convenience Store"),
            school,
            new Emoji(char.ConvertFromUtf32(0x1F3EC), "Department Store"),
            factory,
            new Emoji(char.ConvertFromUtf32(0x1F309), "Bridge at Night"),
            new Emoji("\u26F2", "Fountain"),
            new Emoji(char.ConvertFromUtf32(0x1F6CD) + emojiStyle.Value, "Shopping Bags"),
            new Emoji(char.ConvertFromUtf32(0x1F9FE), "Receipt"),
            new Emoji(char.ConvertFromUtf32(0x1F6D2), "Shopping Cart"),
            new Emoji(char.ConvertFromUtf32(0x1F488), "Barber Pole"),
            new Emoji(char.ConvertFromUtf32(0x1F492), "Wedding"),
            new Emoji(char.ConvertFromUtf32(0x1F5F3) + emojiStyle.Value, "Ballot Box with Ballot"));

        public static readonly EmojiGroup music = new EmojiGroup(
            "Music", "Music",
            new Emoji(char.ConvertFromUtf32(0x1F3BC), "Musical Score"),
            new Emoji(char.ConvertFromUtf32(0x1F3B6), "Musical Notes"),
            new Emoji(char.ConvertFromUtf32(0x1F3B5), "Musical Note"),
            new Emoji(char.ConvertFromUtf32(0x1F3B7), "Saxophone"),
            new Emoji(char.ConvertFromUtf32(0x1F3B8), "Guitar"),
            new Emoji(char.ConvertFromUtf32(0x1F3B9), "Musical Keyboard"),
            new Emoji(char.ConvertFromUtf32(0x1F3BA), "Trumpet"),
            new Emoji(char.ConvertFromUtf32(0x1F3BB), "Violin"),
            new Emoji(char.ConvertFromUtf32(0x1F941), "Drum"),
            new Emoji(char.ConvertFromUtf32(0x1FA97), "Accordion"),
            new Emoji(char.ConvertFromUtf32(0x1FA98), "Long Drum"),
            new Emoji(char.ConvertFromUtf32(0x1FA95), "Banjo"));

        public static readonly Emoji snowflake = new Emoji("\u2744" + emojiStyle.Value, "Snowflake");
        public static readonly Emoji rainbow = new Emoji(char.ConvertFromUtf32(0x1F308), "Rainbow");

        public static readonly EmojiGroup weather = new EmojiGroup(
            "Weather", "Weather",
            new Emoji(char.ConvertFromUtf32(0x1F304), "Sunrise Over Mountains"),
            new Emoji(char.ConvertFromUtf32(0x1F305), "Sunrise"),
            new Emoji(char.ConvertFromUtf32(0x1F306), "Cityscape at Dusk"),
            new Emoji(char.ConvertFromUtf32(0x1F307), "Sunset"),
            new Emoji(char.ConvertFromUtf32(0x1F303), "Night with Stars"),
            new Emoji(char.ConvertFromUtf32(0x1F302), "Closed Umbrella"),
            new Emoji("\u2602" + emojiStyle.Value, "Umbrella"),
            new Emoji("\u2614" + emojiStyle.Value, "Umbrella with Rain Drops"),
            new Emoji("\u2603" + emojiStyle.Value, "Snowman"),
            new Emoji("\u26C4", "Snowman Without Snow"),
            new Emoji("\u2600" + emojiStyle.Value, "Sun"),
            new Emoji("\u2601" + emojiStyle.Value, "Cloud"),
            new Emoji(char.ConvertFromUtf32(0x1F324) + emojiStyle.Value, "Sun Behind Small Cloud"),
            new Emoji("\u26C5", "Sun Behind Cloud"),
            new Emoji(char.ConvertFromUtf32(0x1F325) + emojiStyle.Value, "Sun Behind Large Cloud"),
            new Emoji(char.ConvertFromUtf32(0x1F326) + emojiStyle.Value, "Sun Behind Rain Cloud"),
            new Emoji(char.ConvertFromUtf32(0x1F327) + emojiStyle.Value, "Cloud with Rain"),
            new Emoji(char.ConvertFromUtf32(0x1F328) + emojiStyle.Value, "Cloud with Snow"),
            new Emoji(char.ConvertFromUtf32(0x1F329) + emojiStyle.Value, "Cloud with Lightning"),
            new Emoji("\u26C8" + emojiStyle.Value, "Cloud with Lightning and Rain"),
            snowflake,
            new Emoji(char.ConvertFromUtf32(0x1F300), "Cyclone"),
            new Emoji(char.ConvertFromUtf32(0x1F32A) + emojiStyle.Value, "Tornado"),
            new Emoji(char.ConvertFromUtf32(0x1F32C) + emojiStyle.Value, "Wind Face"),
            new Emoji(char.ConvertFromUtf32(0x1F30A), "Water Wave"),
            new Emoji(char.ConvertFromUtf32(0x1F32B) + emojiStyle.Value, "Fog"),
            new Emoji(char.ConvertFromUtf32(0x1F301), "Foggy"),
            rainbow,
            new Emoji(char.ConvertFromUtf32(0x1F321) + emojiStyle.Value, "Thermometer"));

        public static readonly EmojiGroup astro = new EmojiGroup(
            "Astronomy", "Astronomy",
            new Emoji(char.ConvertFromUtf32(0x1F30C), "Milky Way"),
            new Emoji(char.ConvertFromUtf32(0x1F30D), "Globe Showing Europe-Africa"),
            new Emoji(char.ConvertFromUtf32(0x1F30E), "Globe Showing Americas"),
            new Emoji(char.ConvertFromUtf32(0x1F30F), "Globe Showing Asia-Australia"),
            new Emoji(char.ConvertFromUtf32(0x1F310), "Globe with Meridians"),
            new Emoji(char.ConvertFromUtf32(0x1F311), "New Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F312), "Waxing Crescent Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F313), "First Quarter Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F314), "Waxing Gibbous Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F315), "Full Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F316), "Waning Gibbous Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F317), "Last Quarter Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F318), "Waning Crescent Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F319), "Crescent Moon"),
            new Emoji(char.ConvertFromUtf32(0x1F31A), "New Moon Face"),
            new Emoji(char.ConvertFromUtf32(0x1F31B), "First Quarter Moon Face"),
            new Emoji(char.ConvertFromUtf32(0x1F31C), "Last Quarter Moon Face"),
            new Emoji(char.ConvertFromUtf32(0x1F31D), "Full Moon Face"),
            new Emoji(char.ConvertFromUtf32(0x1F31E), "Sun with Face"),
            new Emoji(char.ConvertFromUtf32(0x1F31F), "Glowing Star"),
            new Emoji(char.ConvertFromUtf32(0x1F320), "Shooting Star"),
            new Emoji("\u2604" + emojiStyle.Value, "Comet"),
            new Emoji(char.ConvertFromUtf32(0x1FA90), "Ringed Planet"));

        public static readonly EmojiGroup finance = new EmojiGroup(
            "Finance", "Finance",
            new Emoji(char.ConvertFromUtf32(0x1F4B0), "Money Bag"),
            new Emoji(char.ConvertFromUtf32(0x1F4B1), "Currency Exchange"),
            new Emoji(char.ConvertFromUtf32(0x1F4B2), "Heavy Dollar Sign"),
            new Emoji(char.ConvertFromUtf32(0x1F4B3), "Credit Card"),
            new Emoji(char.ConvertFromUtf32(0x1F4B4), "Yen Banknote"),
            new Emoji(char.ConvertFromUtf32(0x1F4B5), "Dollar Banknote"),
            new Emoji(char.ConvertFromUtf32(0x1F4B6), "Euro Banknote"),
            new Emoji(char.ConvertFromUtf32(0x1F4B7), "Pound Banknote"),
            new Emoji(char.ConvertFromUtf32(0x1F4B8), "Money with Wings"),
            new Emoji(char.ConvertFromUtf32(0x1FA99), "Coin"),
            new Emoji(char.ConvertFromUtf32(0x1F4B9), "Chart Increasing with Yen"));

        public static readonly EmojiGroup writing = new EmojiGroup(
            "Writing", "Writing",
            new Emoji(char.ConvertFromUtf32(0x1F58A) + emojiStyle.Value, "Pen"),
            new Emoji(char.ConvertFromUtf32(0x1F58B) + emojiStyle.Value, "Fountain Pen"),
            new Emoji(char.ConvertFromUtf32(0x1F58C) + emojiStyle.Value, "Paintbrush"),
            new Emoji(char.ConvertFromUtf32(0x1F58D) + emojiStyle.Value, "Crayon"),
            new Emoji("\u270F" + emojiStyle.Value, "Pencil"),
            new Emoji("\u2712" + emojiStyle.Value, "Black Nib"));

        public static readonly Emoji alembic = new Emoji("\u2697" + emojiStyle.Value, "Alembic");
        public static readonly Emoji gear = new Emoji("\u2699" + emojiStyle.Value, "Gear");
        public static readonly Emoji atomSymbol = new Emoji("\u269B" + emojiStyle.Value, "Atom Symbol");
        public static readonly Emoji keyboard = new Emoji("\u2328" + emojiStyle.Value, "Keyboard");
        public static readonly Emoji telephone = new Emoji("\u260E" + emojiStyle.Value, "Telephone");
        public static readonly Emoji studioMicrophone = new Emoji(char.ConvertFromUtf32(0x1F399) + emojiStyle.Value, "Studio Microphone");
        public static readonly Emoji levelSlider = new Emoji(char.ConvertFromUtf32(0x1F39A) + emojiStyle.Value, "Level Slider");
        public static readonly Emoji controlKnobs = new Emoji(char.ConvertFromUtf32(0x1F39B) + emojiStyle.Value, "Control Knobs");
        public static readonly Emoji movieCamera = new Emoji(char.ConvertFromUtf32(0x1F3A5), "Movie Camera");
        public static readonly Emoji headphone = new Emoji(char.ConvertFromUtf32(0x1F3A7), "Headphone");
        public static readonly Emoji videoGame = new Emoji(char.ConvertFromUtf32(0x1F3AE), "Video Game");
        public static readonly Emoji lightBulb = new Emoji(char.ConvertFromUtf32(0x1F4A1), "Light Bulb");
        public static readonly Emoji computerDisk = new Emoji(char.ConvertFromUtf32(0x1F4BD), "Computer Disk");
        public static readonly Emoji floppyDisk = new Emoji(char.ConvertFromUtf32(0x1F4BE), "Floppy Disk");
        public static readonly Emoji opticalDisk = new Emoji(char.ConvertFromUtf32(0x1F4BF), "Optical Disk");
        public static readonly Emoji dvd = new Emoji(char.ConvertFromUtf32(0x1F4C0), "DVD");
        public static readonly Emoji telephoneReceiver = new Emoji(char.ConvertFromUtf32(0x1F4DE), "Telephone Receiver");
        public static readonly Emoji pager = new Emoji(char.ConvertFromUtf32(0x1F4DF), "Pager");
        public static readonly Emoji faxMachine = new Emoji(char.ConvertFromUtf32(0x1F4E0), "Fax Machine");
        public static readonly Emoji satelliteAntenna = new Emoji(char.ConvertFromUtf32(0x1F4E1), "Satellite Antenna");
        public static readonly Emoji loudspeaker = new Emoji(char.ConvertFromUtf32(0x1F4E2), "Loudspeaker");
        public static readonly Emoji megaphone = new Emoji(char.ConvertFromUtf32(0x1F4E3), "Megaphone");
        public static readonly Emoji mobilePhone = new Emoji(char.ConvertFromUtf32(0x1F4F1), "Mobile Phone");
        public static readonly Emoji mobilePhoneWithArrow = new Emoji(char.ConvertFromUtf32(0x1F4F2), "Mobile Phone with Arrow");
        public static readonly Emoji mobilePhoneVibrating = new Emoji(char.ConvertFromUtf32(0x1F4F3), "Mobile Phone Vibrating");
        public static readonly Emoji mobilePhoneOff = new Emoji(char.ConvertFromUtf32(0x1F4F4), "Mobile Phone Off");
        public static readonly Emoji noMobilePhone = new Emoji(char.ConvertFromUtf32(0x1F4F5), "No Mobile Phone");
        public static readonly Emoji antennaBars = new Emoji(char.ConvertFromUtf32(0x1F4F6), "Antenna Bars");
        public static readonly Emoji camera = new Emoji(char.ConvertFromUtf32(0x1F4F7), "Camera");
        public static readonly Emoji cameraWithFlash = new Emoji(char.ConvertFromUtf32(0x1F4F8), "Camera with Flash");
        public static readonly Emoji videoCamera = new Emoji(char.ConvertFromUtf32(0x1F4F9), "Video Camera");
        public static readonly Emoji television = new Emoji(char.ConvertFromUtf32(0x1F4FA), "Television");
        public static readonly Emoji radio = new Emoji(char.ConvertFromUtf32(0x1F4FB), "Radio");
        public static readonly Emoji videocassette = new Emoji(char.ConvertFromUtf32(0x1F4FC), "Videocassette");
        public static readonly Emoji filmProjector = new Emoji(char.ConvertFromUtf32(0x1F4FD) + emojiStyle.Value, "Film Projector");
        public static readonly Emoji portableStereo = new Emoji(char.ConvertFromUtf32(0x1F4FE) + emojiStyle.Value, "Portable Stereo");
        public static readonly Emoji dimButton = new Emoji(char.ConvertFromUtf32(0x1F505), "Dim Button");
        public static readonly Emoji brightButton = new Emoji(char.ConvertFromUtf32(0x1F506), "Bright Button");
        public static readonly Emoji mutedSpeaker = new Emoji(char.ConvertFromUtf32(0x1F507), "Muted Speaker");
        public static readonly Emoji speakerLowVolume = new Emoji(char.ConvertFromUtf32(0x1F508), "Speaker Low Volume");
        public static readonly Emoji speakerMediumVolume = new Emoji(char.ConvertFromUtf32(0x1F509), "Speaker Medium Volume");
        public static readonly Emoji speakerHighVolume = new Emoji(char.ConvertFromUtf32(0x1F50A), "Speaker High Volume");
        public static readonly Emoji battery = new Emoji(char.ConvertFromUtf32(0x1F50B), "Battery");
        public static readonly Emoji electricPlug = new Emoji(char.ConvertFromUtf32(0x1F50C), "Electric Plug");
        public static readonly Emoji magnifyingGlassTiltedLeft = new Emoji(char.ConvertFromUtf32(0x1F50D), "Magnifying Glass Tilted Left");
        public static readonly Emoji magnifyingGlassTiltedRight = new Emoji(char.ConvertFromUtf32(0x1F50E), "Magnifying Glass Tilted Right");
        public static readonly Emoji lockedWithPen = new Emoji(char.ConvertFromUtf32(0x1F50F), "Locked with Pen");
        public static readonly Emoji lockedWithKey = new Emoji(char.ConvertFromUtf32(0x1F510), "Locked with Key");
        public static readonly Emoji key = new Emoji(char.ConvertFromUtf32(0x1F511), "Key");
        public static readonly Emoji locked = new Emoji(char.ConvertFromUtf32(0x1F512), "Locked");
        public static readonly Emoji unlocked = new Emoji(char.ConvertFromUtf32(0x1F513), "Unlocked");
        public static readonly Emoji bell = new Emoji(char.ConvertFromUtf32(0x1F514), "Bell");
        public static readonly Emoji bellWithSlash = new Emoji(char.ConvertFromUtf32(0x1F515), "Bell with Slash");
        public static readonly Emoji bookmark = new Emoji(char.ConvertFromUtf32(0x1F516), "Bookmark");
        public static readonly Emoji link = new Emoji(char.ConvertFromUtf32(0x1F517), "Link");
        public static readonly Emoji joystick = new Emoji(char.ConvertFromUtf32(0x1F579) + emojiStyle.Value, "Joystick");
        public static readonly Emoji desktopComputer = new Emoji(char.ConvertFromUtf32(0x1F5A5) + emojiStyle.Value, "Desktop Computer");
        public static readonly Emoji printer = new Emoji(char.ConvertFromUtf32(0x1F5A8) + emojiStyle.Value, "Printer");
        public static readonly Emoji computerMouse = new Emoji(char.ConvertFromUtf32(0x1F5B1) + emojiStyle.Value, "Computer Mouse");
        public static readonly Emoji trackball = new Emoji(char.ConvertFromUtf32(0x1F5B2) + emojiStyle.Value, "Trackball");
        public static readonly Emoji blackFolder = new Emoji(char.ConvertFromUtf32(0x1F5BF), "Black Folder");
        public static readonly Emoji folder = new Emoji(char.ConvertFromUtf32(0x1F5C0), "Folder");
        public static readonly Emoji openFolder = new Emoji(char.ConvertFromUtf32(0x1F5C1), "Open Folder");
        public static readonly Emoji cardIndexDividers = new Emoji(char.ConvertFromUtf32(0x1F5C2), "Card Index Dividers");
        public static readonly Emoji cardFileBox = new Emoji(char.ConvertFromUtf32(0x1F5C3), "Card File Box");
        public static readonly Emoji fileCabinet = new Emoji(char.ConvertFromUtf32(0x1F5C4), "File Cabinet");
        public static readonly Emoji emptyNote = new Emoji(char.ConvertFromUtf32(0x1F5C5), "Empty Note");
        public static readonly Emoji emptyNotePage = new Emoji(char.ConvertFromUtf32(0x1F5C6), "Empty Note Page");
        public static readonly Emoji emptyNotePad = new Emoji(char.ConvertFromUtf32(0x1F5C7), "Empty Note Pad");
        public static readonly Emoji note = new Emoji(char.ConvertFromUtf32(0x1F5C8), "Note");
        public static readonly Emoji notePage = new Emoji(char.ConvertFromUtf32(0x1F5C9), "Note Page");
        public static readonly Emoji notePad = new Emoji(char.ConvertFromUtf32(0x1F5CA), "Note Pad");
        public static readonly Emoji emptyDocument = new Emoji(char.ConvertFromUtf32(0x1F5CB), "Empty Document");
        public static readonly Emoji emptyPage = new Emoji(char.ConvertFromUtf32(0x1F5CC), "Empty Page");
        public static readonly Emoji emptyPages = new Emoji(char.ConvertFromUtf32(0x1F5CD), "Empty Pages");
        public static readonly Emoji documentIcon = new Emoji(char.ConvertFromUtf32(0x1F5CE), "Document");
        public static readonly Emoji page = new Emoji(char.ConvertFromUtf32(0x1F5CF), "Page");
        public static readonly Emoji pages = new Emoji(char.ConvertFromUtf32(0x1F5D0), "Pages");
        public static readonly Emoji wastebasket = new Emoji(char.ConvertFromUtf32(0x1F5D1), "Wastebasket");
        public static readonly Emoji spiralNotePad = new Emoji(char.ConvertFromUtf32(0x1F5D2), "Spiral Note Pad");
        public static readonly Emoji spiralCalendar = new Emoji(char.ConvertFromUtf32(0x1F5D3), "Spiral Calendar");
        public static readonly Emoji desktopWindow = new Emoji(char.ConvertFromUtf32(0x1F5D4), "Desktop Window");
        public static readonly Emoji minimize = new Emoji(char.ConvertFromUtf32(0x1F5D5), "Minimize");
        public static readonly Emoji maximize = new Emoji(char.ConvertFromUtf32(0x1F5D6), "Maximize");
        public static readonly Emoji overlap = new Emoji(char.ConvertFromUtf32(0x1F5D7), "Overlap");
        public static readonly Emoji reload = new Emoji(char.ConvertFromUtf32(0x1F5D8), "Reload");
        public static readonly Emoji close = new Emoji(char.ConvertFromUtf32(0x1F5D9), "Close");
        public static readonly Emoji increaseFontSize = new Emoji(char.ConvertFromUtf32(0x1F5DA), "Increase Font Size");
        public static readonly Emoji decreaseFontSize = new Emoji(char.ConvertFromUtf32(0x1F5DB), "Decrease Font Size");
        public static readonly Emoji compression = new Emoji(char.ConvertFromUtf32(0x1F5DC), "Compression");
        public static readonly Emoji oldKey = new Emoji(char.ConvertFromUtf32(0x1F5DD), "Old Key");
        public static readonly EmojiGroup tech = new EmojiGroup(
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

        public static readonly EmojiGroup mail = new EmojiGroup(
            "Mail", "Mail",
            new Emoji(char.ConvertFromUtf32(0x1F4E4), "Outbox Tray"),
            new Emoji(char.ConvertFromUtf32(0x1F4E5), "Inbox Tray"),
            new Emoji(char.ConvertFromUtf32(0x1F4E6), "Package"),
            new Emoji(char.ConvertFromUtf32(0x1F4E7), "E-Mail"),
            new Emoji(char.ConvertFromUtf32(0x1F4E8), "Incoming Envelope"),
            new Emoji(char.ConvertFromUtf32(0x1F4E9), "Envelope with Arrow"),
            new Emoji(char.ConvertFromUtf32(0x1F4EA), "Closed Mailbox with Lowered Flag"),
            new Emoji(char.ConvertFromUtf32(0x1F4EB), "Closed Mailbox with Raised Flag"),
            new Emoji(char.ConvertFromUtf32(0x1F4EC), "Open Mailbox with Raised Flag"),
            new Emoji(char.ConvertFromUtf32(0x1F4ED), "Open Mailbox with Lowered Flag"),
            new Emoji(char.ConvertFromUtf32(0x1F4EE), "Postbox"),
            new Emoji(char.ConvertFromUtf32(0x1F4EF), "Postal Horn"));

        public static readonly EmojiGroup celebration = new EmojiGroup(
            "Celebration", "Celebration",
            new Emoji(char.ConvertFromUtf32(0x1F380), "Ribbon"),
            new Emoji(char.ConvertFromUtf32(0x1F381), "Wrapped Gift"),
            new Emoji(char.ConvertFromUtf32(0x1F383), "Jack-O-Lantern"),
            new Emoji(char.ConvertFromUtf32(0x1F384), "Christmas Tree"),
            new Emoji(char.ConvertFromUtf32(0x1F9E8), "Firecracker"),
            new Emoji(char.ConvertFromUtf32(0x1F386), "Fireworks"),
            new Emoji(char.ConvertFromUtf32(0x1F387), "Sparkler"),
            new Emoji("\u2728", "Sparkles"),
            new Emoji("\u2747" + emojiStyle.Value, "Sparkle"),
            new Emoji(char.ConvertFromUtf32(0x1F388), "Balloon"),
            new Emoji(char.ConvertFromUtf32(0x1F389), "Party Popper"),
            new Emoji(char.ConvertFromUtf32(0x1F38A), "Confetti Ball"),
            new Emoji(char.ConvertFromUtf32(0x1F38B), "Tanabata Tree"),
            new Emoji(char.ConvertFromUtf32(0x1F38D), "Pine Decoration"),
            new Emoji(char.ConvertFromUtf32(0x1F38E), "Japanese Dolls"),
            new Emoji(char.ConvertFromUtf32(0x1F38F), "Carp Streamer"),
            new Emoji(char.ConvertFromUtf32(0x1F390), "Wind Chime"),
            new Emoji(char.ConvertFromUtf32(0x1F391), "Moon Viewing Ceremony"),
            new Emoji(char.ConvertFromUtf32(0x1F392), "Backpack"),
            graduationCap,
            new Emoji(char.ConvertFromUtf32(0x1F9E7), "Red Envelope"),
            new Emoji(char.ConvertFromUtf32(0x1F3EE), "Red Paper Lantern"),
            new Emoji(char.ConvertFromUtf32(0x1F396) + emojiStyle.Value, "Military Medal"));

        public static readonly EmojiGroup tools = new EmojiGroup(
            "Tools", "Tools",
            new Emoji(char.ConvertFromUtf32(0x1F3A3), "Fishing Pole"),
            new Emoji(char.ConvertFromUtf32(0x1F526), "Flashlight"),
            wrench,
            new Emoji(char.ConvertFromUtf32(0x1F528), "Hammer"),
            new Emoji(char.ConvertFromUtf32(0x1F529), "Nut and Bolt"),
            new Emoji(char.ConvertFromUtf32(0x1F6E0) + emojiStyle.Value, "Hammer and Wrench"),
            new Emoji(char.ConvertFromUtf32(0x1F9ED), "Compass"),
            new Emoji(char.ConvertFromUtf32(0x1F9EF), "Fire Extinguisher"),
            new Emoji(char.ConvertFromUtf32(0x1F9F0), "Toolbox"),
            new Emoji(char.ConvertFromUtf32(0x1F9F1), "Brick"),
            new Emoji(char.ConvertFromUtf32(0x1FA93), "Axe"),
            new Emoji("\u2692" + emojiStyle.Value, "Hammer and Pick"),
            new Emoji("\u26CF" + emojiStyle.Value, "Pick"),
            new Emoji("\u26D1" + emojiStyle.Value, "Rescue Worker’s Helmet"),
            new Emoji("\u26D3" + emojiStyle.Value, "Chains"),
            compression);

        public static readonly EmojiGroup office = new EmojiGroup(
            "Office", "Office",
            new Emoji(char.ConvertFromUtf32(0x1F4C1), "File Folder"),
            new Emoji(char.ConvertFromUtf32(0x1F4C2), "Open File Folder"),
            new Emoji(char.ConvertFromUtf32(0x1F4C3), "Page with Curl"),
            new Emoji(char.ConvertFromUtf32(0x1F4C4), "Page Facing Up"),
            new Emoji(char.ConvertFromUtf32(0x1F4C5), "Calendar"),
            new Emoji(char.ConvertFromUtf32(0x1F4C6), "Tear-Off Calendar"),
            new Emoji(char.ConvertFromUtf32(0x1F4C7), "Card Index"),
            cardIndexDividers,
            cardFileBox,
            fileCabinet,
            wastebasket,
            spiralNotePad,
            spiralCalendar,
            new Emoji(char.ConvertFromUtf32(0x1F4C8), "Chart Increasing"),
            new Emoji(char.ConvertFromUtf32(0x1F4C9), "Chart Decreasing"),
            new Emoji(char.ConvertFromUtf32(0x1F4CA), "Bar Chart"),
            new Emoji(char.ConvertFromUtf32(0x1F4CB), "Clipboard"),
            new Emoji(char.ConvertFromUtf32(0x1F4CC), "Pushpin"),
            new Emoji(char.ConvertFromUtf32(0x1F4CD), "Round Pushpin"),
            new Emoji(char.ConvertFromUtf32(0x1F4CE), "Paperclip"),
            new Emoji(char.ConvertFromUtf32(0x1F587) + emojiStyle.Value, "Linked Paperclips"),
            new Emoji(char.ConvertFromUtf32(0x1F4CF), "Straight Ruler"),
            new Emoji(char.ConvertFromUtf32(0x1F4D0), "Triangular Ruler"),
            new Emoji(char.ConvertFromUtf32(0x1F4D1), "Bookmark Tabs"),
            new Emoji(char.ConvertFromUtf32(0x1F4D2), "Ledger"),
            new Emoji(char.ConvertFromUtf32(0x1F4D3), "Notebook"),
            new Emoji(char.ConvertFromUtf32(0x1F4D4), "Notebook with Decorative Cover"),
            new Emoji(char.ConvertFromUtf32(0x1F4D5), "Closed Book"),
            new Emoji(char.ConvertFromUtf32(0x1F4D6), "Open Book"),
            new Emoji(char.ConvertFromUtf32(0x1F4D7), "Green Book"),
            new Emoji(char.ConvertFromUtf32(0x1F4D8), "Blue Book"),
            new Emoji(char.ConvertFromUtf32(0x1F4D9), "Orange Book"),
            new Emoji(char.ConvertFromUtf32(0x1F4DA), "Books"),
            new Emoji(char.ConvertFromUtf32(0x1F4DB), "Name Badge"),
            new Emoji(char.ConvertFromUtf32(0x1F4DC), "Scroll"),
            new Emoji(char.ConvertFromUtf32(0x1F4DD), "Memo"),
            new Emoji("\u2702" + emojiStyle.Value, "Scissors"),
            new Emoji("\u2709" + emojiStyle.Value, "Envelope"));

        public static readonly EmojiGroup signs = new EmojiGroup(
            "Signs", "Signs",
            new Emoji(char.ConvertFromUtf32(0x1F3A6), "Cinema"),
            noMobilePhone,
            new Emoji(char.ConvertFromUtf32(0x1F51E), "No One Under Eighteen"),
            new Emoji(char.ConvertFromUtf32(0x1F6AB), "Prohibited"),
            new Emoji(char.ConvertFromUtf32(0x1F6AC), "Cigarette"),
            new Emoji(char.ConvertFromUtf32(0x1F6AD), "No Smoking"),
            new Emoji(char.ConvertFromUtf32(0x1F6AE), "Litter in Bin Sign"),
            new Emoji(char.ConvertFromUtf32(0x1F6AF), "No Littering"),
            new Emoji(char.ConvertFromUtf32(0x1F6B0), "Potable Water"),
            new Emoji(char.ConvertFromUtf32(0x1F6B1), "Non-Potable Water"),
            new Emoji(char.ConvertFromUtf32(0x1F6B3), "No Bicycles"),
            new Emoji(char.ConvertFromUtf32(0x1F6B7), "No Pedestrians"),
            new Emoji(char.ConvertFromUtf32(0x1F6B8), "Children Crossing"),
            new Emoji(char.ConvertFromUtf32(0x1F6B9), "Men’s Room"),
            new Emoji(char.ConvertFromUtf32(0x1F6BA), "Women’s Room"),
            new Emoji(char.ConvertFromUtf32(0x1F6BB), "Restroom"),
            new Emoji(char.ConvertFromUtf32(0x1F6BC), "Baby Symbol"),
            new Emoji(char.ConvertFromUtf32(0x1F6BE), "Water Closet"),
            new Emoji(char.ConvertFromUtf32(0x1F6C2), "Passport Control"),
            new Emoji(char.ConvertFromUtf32(0x1F6C3), "Customs"),
            new Emoji(char.ConvertFromUtf32(0x1F6C4), "Baggage Claim"),
            new Emoji(char.ConvertFromUtf32(0x1F6C5), "Left Luggage"),
            new Emoji(char.ConvertFromUtf32(0x1F17F) + emojiStyle.Value, "Parking Button"),
            new Emoji("\u267F", "Wheelchair Symbol"),
            new Emoji("\u2622" + emojiStyle.Value, "Radioactive"),
            new Emoji("\u2623" + emojiStyle.Value, "Biohazard"),
            new Emoji("\u26A0" + emojiStyle.Value, "Warning"),
            new Emoji("\u26A1", "High Voltage"),
            new Emoji("\u26D4", "No Entry"),
            new Emoji("\u267B" + emojiStyle.Value, "Recycling Symbol"),
            female,
            male,
            transgender);

        public static readonly EmojiGroup religion = new EmojiGroup(
            "Religion", "Religion",
            new Emoji(char.ConvertFromUtf32(0x1F52F), "Dotted Six-Pointed Star"),
            new Emoji("\u2721" + emojiStyle.Value, "Star of David"),
            new Emoji(char.ConvertFromUtf32(0x1F549) + emojiStyle.Value, "Om"),
            new Emoji(char.ConvertFromUtf32(0x1F54B), "Kaaba"),
            new Emoji(char.ConvertFromUtf32(0x1F54C), "Mosque"),
            new Emoji(char.ConvertFromUtf32(0x1F54D), "Synagogue"),
            new Emoji(char.ConvertFromUtf32(0x1F54E), "Menorah"),
            new Emoji(char.ConvertFromUtf32(0x1F6D0), "Place of Worship"),
            new Emoji(char.ConvertFromUtf32(0x1F6D5), "Hindu Temple"),
            new Emoji("\u2626" + emojiStyle.Value, "Orthodox Cross"),
            new Emoji("\u271D" + emojiStyle.Value, "Latin Cross"),
            new Emoji("\u262A" + emojiStyle.Value, "Star and Crescent"),
            new Emoji("\u262E" + emojiStyle.Value, "Peace Symbol"),
            new Emoji("\u262F" + emojiStyle.Value, "Yin Yang"),
            new Emoji("\u2638" + emojiStyle.Value, "Wheel of Dharma"),
            new Emoji("\u267E" + emojiStyle.Value, "Infinity"),
            new Emoji(char.ConvertFromUtf32(0x1FA94), "Diya Lamp"),
            new Emoji("\u26E9" + emojiStyle.Value, "Shinto Shrine"),
            new Emoji("\u26EA", "Church"),
            new Emoji("\u2734" + emojiStyle.Value, "Eight-Pointed Star"),
            new Emoji(char.ConvertFromUtf32(0x1F4FF), "Prayer Beads"));

        public static readonly Emoji door = new Emoji(char.ConvertFromUtf32(0x1F6AA), "Door");
        public static readonly EmojiGroup household = new EmojiGroup(
            "Household", "Household",
            new Emoji(char.ConvertFromUtf32(0x1F484), "Lipstick"),
            new Emoji(char.ConvertFromUtf32(0x1F48D), "Ring"),
            new Emoji(char.ConvertFromUtf32(0x1F48E), "Gem Stone"),
            new Emoji(char.ConvertFromUtf32(0x1F4F0), "Newspaper"),
            key,
            new Emoji(char.ConvertFromUtf32(0x1F525), "Fire"),
            new Emoji(char.ConvertFromUtf32(0x1F52B), "Pistol"),
            new Emoji(char.ConvertFromUtf32(0x1F56F) + emojiStyle.Value, "Candle"),
            new Emoji(char.ConvertFromUtf32(0x1F5BC) + emojiStyle.Value, "Framed Picture"),
            oldKey,
            new Emoji(char.ConvertFromUtf32(0x1F5DE) + emojiStyle.Value, "Rolled-Up Newspaper"),
            new Emoji(char.ConvertFromUtf32(0x1F5FA) + emojiStyle.Value, "World Map"),
            door,
            new Emoji(char.ConvertFromUtf32(0x1F6BD), "Toilet"),
            new Emoji(char.ConvertFromUtf32(0x1F6BF), "Shower"),
            new Emoji(char.ConvertFromUtf32(0x1F6C1), "Bathtub"),
            new Emoji(char.ConvertFromUtf32(0x1F6CB) + emojiStyle.Value, "Couch and Lamp"),
            new Emoji(char.ConvertFromUtf32(0x1F6CF) + emojiStyle.Value, "Bed"),
            new Emoji(char.ConvertFromUtf32(0x1F9F4), "Lotion Bottle"),
            new Emoji(char.ConvertFromUtf32(0x1F9F5), "Thread"),
            new Emoji(char.ConvertFromUtf32(0x1F9F6), "Yarn"),
            new Emoji(char.ConvertFromUtf32(0x1F9F7), "Safety Pin"),
            new Emoji(char.ConvertFromUtf32(0x1F9F8), "Teddy Bear"),
            new Emoji(char.ConvertFromUtf32(0x1F9F9), "Broom"),
            new Emoji(char.ConvertFromUtf32(0x1F9FA), "Basket"),
            new Emoji(char.ConvertFromUtf32(0x1F9FB), "Roll of Paper"),
            new Emoji(char.ConvertFromUtf32(0x1F9FC), "Soap"),
            new Emoji(char.ConvertFromUtf32(0x1F9FD), "Sponge"),
            new Emoji(char.ConvertFromUtf32(0x1FA91), "Chair"),
            new Emoji(char.ConvertFromUtf32(0x1FA92), "Razor"),
            new Emoji(char.ConvertFromUtf32(0x1F397) + emojiStyle.Value, "Reminder Ribbon"));

        public static readonly EmojiGroup activities = new EmojiGroup(
            "Activities", "Activities",
            new Emoji(char.ConvertFromUtf32(0x1F39E) + emojiStyle.Value, "Film Frames"),
            new Emoji(char.ConvertFromUtf32(0x1F39F) + emojiStyle.Value, "Admission Tickets"),
            new Emoji(char.ConvertFromUtf32(0x1F3A0), "Carousel Horse"),
            new Emoji(char.ConvertFromUtf32(0x1F3A1), "Ferris Wheel"),
            new Emoji(char.ConvertFromUtf32(0x1F3A2), "Roller Coaster"),
            artistPalette,
            new Emoji(char.ConvertFromUtf32(0x1F3AA), "Circus Tent"),
            new Emoji(char.ConvertFromUtf32(0x1F3AB), "Ticket"),
            new Emoji(char.ConvertFromUtf32(0x1F3AC), "Clapper Board"),
            new Emoji(char.ConvertFromUtf32(0x1F3AD), "Performing Arts"));

        public static readonly EmojiGroup travel = new EmojiGroup(
            "Travel", "Travel",
            new Emoji(char.ConvertFromUtf32(0x1F3F7) + emojiStyle.Value, "Label"),
            new Emoji(char.ConvertFromUtf32(0x1F30B), "Volcano"),
            new Emoji(char.ConvertFromUtf32(0x1F3D4) + emojiStyle.Value, "Snow-Capped Mountain"),
            new Emoji("\u26F0" + emojiStyle.Value, "Mountain"),
            new Emoji(char.ConvertFromUtf32(0x1F3D5) + emojiStyle.Value, "Camping"),
            new Emoji(char.ConvertFromUtf32(0x1F3D6) + emojiStyle.Value, "Beach with Umbrella"),
            new Emoji("\u26F1" + emojiStyle.Value, "Umbrella on Ground"),
            new Emoji(char.ConvertFromUtf32(0x1F3EF), "Japanese Castle"),
            new Emoji(char.ConvertFromUtf32(0x1F463), "Footprints"),
            new Emoji(char.ConvertFromUtf32(0x1F5FB), "Mount Fuji"),
            new Emoji(char.ConvertFromUtf32(0x1F5FC), "Tokyo Tower"),
            new Emoji(char.ConvertFromUtf32(0x1F5FD), "Statue of Liberty"),
            new Emoji(char.ConvertFromUtf32(0x1F5FE), "Map of Japan"),
            new Emoji(char.ConvertFromUtf32(0x1F5FF), "Moai"),
            new Emoji(char.ConvertFromUtf32(0x1F6CE) + emojiStyle.Value, "Bellhop Bell"),
            new Emoji(char.ConvertFromUtf32(0x1F9F3), "Luggage"),
            new Emoji("\u26F3", "Flag in Hole"),
            new Emoji("\u26FA", "Tent"),
            new Emoji("\u2668" + emojiStyle.Value, "Hot Springs"));

        public static readonly EmojiGroup medieval = new EmojiGroup(
            "Medieval", "Medieval",
            new Emoji(char.ConvertFromUtf32(0x1F3F0), "Castle"),
            new Emoji(char.ConvertFromUtf32(0x1F3F9), "Bow and Arrow"),
            crown,
            new Emoji(char.ConvertFromUtf32(0x1F531), "Trident Emblem"),
            new Emoji(char.ConvertFromUtf32(0x1F5E1) + emojiStyle.Value, "Dagger"),
            new Emoji(char.ConvertFromUtf32(0x1F6E1) + emojiStyle.Value, "Shield"),
            new Emoji(char.ConvertFromUtf32(0x1F52E), "Crystal Ball"),
            new Emoji("\u2694" + emojiStyle.Value, "Crossed Swords"),
            new Emoji("\u269C" + emojiStyle.Value, "Fleur-de-lis"));

        public static readonly Emoji doubleExclamationMark = new Emoji("\u203C" + emojiStyle.Value, "Double Exclamation Mark");
        public static readonly Emoji interrobang = new Emoji("\u2049" + emojiStyle.Value, "Exclamation Question Mark");
        public static readonly Emoji information = new Emoji("\u2139" + emojiStyle.Value, "Information");
        public static readonly Emoji circledM = new Emoji("\u24C2" + emojiStyle.Value, "Circled M");
        public static readonly Emoji checkMarkButton = new Emoji("\u2705", "Check Mark Button");
        public static readonly Emoji checkMark = new Emoji("\u2714" + emojiStyle.Value, "Check Mark");
        public static readonly Emoji eightSpokedAsterisk = new Emoji("\u2733" + emojiStyle.Value, "Eight-Spoked Asterisk");
        public static readonly Emoji crossMark = new Emoji("\u274C", "Cross Mark");
        public static readonly Emoji crossMarkButton = new Emoji("\u274E", "Cross Mark Button");
        public static readonly Emoji questionMark = new Emoji("\u2753", "Question Mark");
        public static readonly Emoji whiteQuestionMark = new Emoji("\u2754", "White Question Mark");
        public static readonly Emoji whiteExclamationMark = new Emoji("\u2755", "White Exclamation Mark");
        public static readonly Emoji exclamationMark = new Emoji("\u2757", "Exclamation Mark");
        public static readonly Emoji curlyLoop = new Emoji("\u27B0", "Curly Loop");
        public static readonly Emoji doubleCurlyLoop = new Emoji("\u27BF", "Double Curly Loop");
        public static readonly Emoji wavyDash = new Emoji("\u3030" + emojiStyle.Value, "Wavy Dash");
        public static readonly Emoji partAlternationMark = new Emoji("\u303D" + emojiStyle.Value, "Part Alternation Mark");
        public static readonly Emoji tradeMark = new Emoji("\u2122" + emojiStyle.Value, "Trade Mark");
        public static readonly Emoji copyright = new Emoji("\u00A9" + emojiStyle.Value, "Copyright");
        public static readonly Emoji registered = new Emoji("\u00AE" + emojiStyle.Value, "Registered");
        public static readonly Emoji squareFourCourners = new Emoji("\u26F6" + emojiStyle.Value, "Square: Four Corners");

        public static readonly EmojiGroup marks = new EmojiGroup(
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

        public static readonly Emoji droplet = new Emoji(char.ConvertFromUtf32(0x1F4A7), "Droplet");
        public static readonly Emoji dropOfBlood = new Emoji(char.ConvertFromUtf32(0x1FA78), "Drop of Blood");
        public static readonly Emoji adhesiveBandage = new Emoji(char.ConvertFromUtf32(0x1FA79), "Adhesive Bandage");
        public static readonly Emoji stethoscope = new Emoji(char.ConvertFromUtf32(0x1FA7A), "Stethoscope");
        public static readonly Emoji syringe = new Emoji(char.ConvertFromUtf32(0x1F489), "Syringe");
        public static readonly Emoji pill = new Emoji(char.ConvertFromUtf32(0x1F48A), "Pill");
        public static readonly Emoji testTube = new Emoji(char.ConvertFromUtf32(0x1F9EA), "Test Tube");
        public static readonly Emoji petriDish = new Emoji(char.ConvertFromUtf32(0x1F9EB), "Petri Dish");
        public static readonly Emoji dna = new Emoji(char.ConvertFromUtf32(0x1F9EC), "DNA");
        public static readonly Emoji abacus = new Emoji(char.ConvertFromUtf32(0x1F9EE), "Abacus");
        public static readonly Emoji magnet = new Emoji(char.ConvertFromUtf32(0x1F9F2), "Magnet");
        public static readonly Emoji telescope = new Emoji(char.ConvertFromUtf32(0x1F52D), "Telescope");

        public static readonly EmojiGroup science = new EmojiGroup(
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

        public static readonly Emoji whiteChessKing = new Emoji("\u2654", "White Chess King");
        public static readonly Emoji whiteChessQueen = new Emoji("\u2655", "White Chess Queen");
        public static readonly Emoji whiteChessRook = new Emoji("\u2656", "White Chess Rook");
        public static readonly Emoji whiteChessBishop = new Emoji("\u2657", "White Chess Bishop");
        public static readonly Emoji whiteChessKnight = new Emoji("\u2658", "White Chess Knight");
        public static readonly Emoji whiteChessPawn = new Emoji("\u2659", "White Chess Pawn");
        public static readonly EmojiGroup whiteChessPieces = new EmojiGroup(
            whiteChessKing.Value + whiteChessQueen.Value + whiteChessRook.Value + whiteChessBishop.Value + whiteChessKnight.Value + whiteChessPawn.Value,
            "White Chess Pieces",
            whiteChessKing,
            whiteChessQueen,
            whiteChessRook,
            whiteChessBishop,
            whiteChessKnight,
            whiteChessPawn);

        public static readonly Emoji blackChessKing = new Emoji("\u265A", "Black Chess King");
        public static readonly Emoji blackChessQueen = new Emoji("\u265B", "Black Chess Queen");
        public static readonly Emoji blackChessRook = new Emoji("\u265C", "Black Chess Rook");
        public static readonly Emoji blackChessBishop = new Emoji("\u265D", "Black Chess Bishop");
        public static readonly Emoji blackChessKnight = new Emoji("\u265E", "Black Chess Knight");
        public static readonly Emoji blackChessPawn = new Emoji("\u265F", "Black Chess Pawn");
        public static readonly EmojiGroup blackChessPieces = new EmojiGroup(
            blackChessKing.Value + blackChessQueen.Value + blackChessRook.Value + blackChessBishop.Value + blackChessKnight.Value + blackChessPawn.Value,
            "Black Chess Pieces",
            blackChessKing,
            blackChessQueen,
            blackChessRook,
            blackChessBishop,
            blackChessKnight,
            blackChessPawn);

        public static readonly EmojiGroup chessPawns = new EmojiGroup(
            whiteChessPawn.Value + blackChessPawn.Value,
            "Chess Pawns",
            whiteChessPawn,
            blackChessPawn);

        public static readonly EmojiGroup chessRooks = new EmojiGroup(
            whiteChessRook.Value + blackChessRook.Value,
            "Chess Rooks",
            whiteChessRook,
            blackChessRook);

        public static readonly EmojiGroup chessBishops = new EmojiGroup(
            whiteChessBishop.Value + blackChessBishop.Value,
            "Chess Bishops",
            whiteChessBishop,
            blackChessBishop);

        public static readonly EmojiGroup chessKnights = new EmojiGroup(
            whiteChessKnight.Value + blackChessKnight.Value,
            "Chess Knights",
            whiteChessKnight,
            blackChessKnight);

        public static readonly EmojiGroup chessQueens = new EmojiGroup(
            whiteChessQueen.Value + blackChessQueen.Value,
            "Chess Queens",
            whiteChessQueen,
            blackChessQueen);

        public static readonly EmojiGroup chessKings = new EmojiGroup(
            whiteChessKing.Value + blackChessKing.Value,
            "Chess Kings",
            whiteChessKing,
            blackChessKing);

        public static readonly EmojiGroup chess = new EmojiGroup(
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

        public static readonly Emoji dice1 = new Emoji("\u2680", "Dice: Side 1");
        public static readonly Emoji dice2 = new Emoji("\u2681", "Dice: Side 2");
        public static readonly Emoji dice3 = new Emoji("\u2682", "Dice: Side 3");
        public static readonly Emoji dice4 = new Emoji("\u2683", "Dice: Side 4");
        public static readonly Emoji dice5 = new Emoji("\u2684", "Dice: Side 5");
        public static readonly Emoji dice6 = new Emoji("\u2685", "Dice: Side 6");
        public static readonly EmojiGroup dice = new EmojiGroup(
            "Dice",
            "Dice",
            dice1,
            dice2,
            dice3,
            dice4,
            dice5,
            dice6);

        public static readonly Emoji crossedFlags = new Emoji(char.ConvertFromUtf32(0x1F38C), "Crossed Flags");
        public static readonly Emoji chequeredFlag = new Emoji(char.ConvertFromUtf32(0x1F3C1), "Chequered Flag");
        public static readonly Emoji whiteFlag = new Emoji(char.ConvertFromUtf32(0x1F3F3) + emojiStyle.Value, "White Flag");
        public static readonly Emoji blackFlag = new Emoji(char.ConvertFromUtf32(0x1F3F4), "Black Flag");
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
        public static readonly Emoji triangularFlag = new Emoji(char.ConvertFromUtf32(0x1F6A9), "Triangular Flag");

        public static readonly EmojiGroup flags = new EmojiGroup(
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

        public static readonly Emoji cat = new Emoji(char.ConvertFromUtf32(0x1F408), "Cat");
        public static readonly Emoji dog = new Emoji(char.ConvertFromUtf32(0x1F415), "Dog");
        public static readonly Emoji bear = new Emoji(char.ConvertFromUtf32(0x1F43B), "Bear");
        public static readonly Emoji blackCat = Join(cat, blackLargeSquare, "Black Cat");
        public static readonly Emoji serviceDog = Join(dog, safetyVest, "Service Dog");
        public static readonly Emoji polarBear = Join(bear, snowflake, "Polar Bear");

        public static readonly EmojiGroup animals = new EmojiGroup(
            "Animals", "Animals and insects",
            new Emoji(char.ConvertFromUtf32(0x1F400), "Rat"),
            new Emoji(char.ConvertFromUtf32(0x1F401), "Mouse"),
            new Emoji(char.ConvertFromUtf32(0x1F402), "Ox"),
            new Emoji(char.ConvertFromUtf32(0x1F403), "Water Buffalo"),
            new Emoji(char.ConvertFromUtf32(0x1F404), "Cow"),
            new Emoji(char.ConvertFromUtf32(0x1F405), "Tiger"),
            new Emoji(char.ConvertFromUtf32(0x1F406), "Leopard"),
            new Emoji(char.ConvertFromUtf32(0x1F407), "Rabbit"),
            cat,
            blackCat,
            new Emoji(char.ConvertFromUtf32(0x1F409), "Dragon"),
            new Emoji(char.ConvertFromUtf32(0x1F40A), "Crocodile"),
            new Emoji(char.ConvertFromUtf32(0x1F40B), "Whale"),
            new Emoji(char.ConvertFromUtf32(0x1F40C), "Snail"),
            new Emoji(char.ConvertFromUtf32(0x1F40D), "Snake"),
            new Emoji(char.ConvertFromUtf32(0x1F40E), "Horse"),
            new Emoji(char.ConvertFromUtf32(0x1F40F), "Ram"),
            new Emoji(char.ConvertFromUtf32(0x1F410), "Goat"),
            new Emoji(char.ConvertFromUtf32(0x1F411), "Ewe"),
            new Emoji(char.ConvertFromUtf32(0x1F412), "Monkey"),
            new Emoji(char.ConvertFromUtf32(0x1F413), "Rooster"),
            new Emoji(char.ConvertFromUtf32(0x1F414), "Chicken"),
            dog,
            serviceDog,
            new Emoji(char.ConvertFromUtf32(0x1F416), "Pig"),
            new Emoji(char.ConvertFromUtf32(0x1F417), "Boar"),
            new Emoji(char.ConvertFromUtf32(0x1F418), "Elephant"),
            new Emoji(char.ConvertFromUtf32(0x1F419), "Octopus"),
            new Emoji(char.ConvertFromUtf32(0x1F41A), "Spiral Shell"),
            new Emoji(char.ConvertFromUtf32(0x1F41B), "Bug"),
            new Emoji(char.ConvertFromUtf32(0x1F41C), "Ant"),
            new Emoji(char.ConvertFromUtf32(0x1F41D), "Honeybee"),
            new Emoji(char.ConvertFromUtf32(0x1F41E), "Lady Beetle"),
            new Emoji(char.ConvertFromUtf32(0x1F41F), "Fish"),
            new Emoji(char.ConvertFromUtf32(0x1F420), "Tropical Fish"),
            new Emoji(char.ConvertFromUtf32(0x1F421), "Blowfish"),
            new Emoji(char.ConvertFromUtf32(0x1F422), "Turtle"),
            new Emoji(char.ConvertFromUtf32(0x1F423), "Hatching Chick"),
            new Emoji(char.ConvertFromUtf32(0x1F424), "Baby Chick"),
            new Emoji(char.ConvertFromUtf32(0x1F425), "Front-Facing Baby Chick"),
            new Emoji(char.ConvertFromUtf32(0x1F426), "Bird"),
            new Emoji(char.ConvertFromUtf32(0x1F427), "Penguin"),
            new Emoji(char.ConvertFromUtf32(0x1F428), "Koala"),
            new Emoji(char.ConvertFromUtf32(0x1F429), "Poodle"),
            new Emoji(char.ConvertFromUtf32(0x1F42A), "Camel"),
            new Emoji(char.ConvertFromUtf32(0x1F42B), "Two-Hump Camel"),
            new Emoji(char.ConvertFromUtf32(0x1F42C), "Dolphin"),
            new Emoji(char.ConvertFromUtf32(0x1F42D), "Mouse Face"),
            new Emoji(char.ConvertFromUtf32(0x1F42E), "Cow Face"),
            new Emoji(char.ConvertFromUtf32(0x1F42F), "Tiger Face"),
            new Emoji(char.ConvertFromUtf32(0x1F430), "Rabbit Face"),
            new Emoji(char.ConvertFromUtf32(0x1F431), "Cat Face"),
            new Emoji(char.ConvertFromUtf32(0x1F432), "Dragon Face"),
            new Emoji(char.ConvertFromUtf32(0x1F433), "Spouting Whale"),
            new Emoji(char.ConvertFromUtf32(0x1F434), "Horse Face"),
            new Emoji(char.ConvertFromUtf32(0x1F435), "Monkey Face"),
            new Emoji(char.ConvertFromUtf32(0x1F436), "Dog Face"),
            new Emoji(char.ConvertFromUtf32(0x1F437), "Pig Face"),
            new Emoji(char.ConvertFromUtf32(0x1F438), "Frog"),
            new Emoji(char.ConvertFromUtf32(0x1F439), "Hamster"),
            new Emoji(char.ConvertFromUtf32(0x1F43A), "Wolf"),
            bear,
            polarBear,
            new Emoji(char.ConvertFromUtf32(0x1F43C), "Panda"),
            new Emoji(char.ConvertFromUtf32(0x1F43D), "Pig Nose"),
            new Emoji(char.ConvertFromUtf32(0x1F43E), "Paw Prints"),
            new Emoji(char.ConvertFromUtf32(0x1F43F) + emojiStyle.Value, "Chipmunk"),
            new Emoji(char.ConvertFromUtf32(0x1F54A) + emojiStyle.Value, "Dove"),
            new Emoji(char.ConvertFromUtf32(0x1F577) + emojiStyle.Value, "Spider"),
            new Emoji(char.ConvertFromUtf32(0x1F578) + emojiStyle.Value, "Spider Web"),
            new Emoji(char.ConvertFromUtf32(0x1F981), "Lion"),
            new Emoji(char.ConvertFromUtf32(0x1F982), "Scorpion"),
            new Emoji(char.ConvertFromUtf32(0x1F983), "Turkey"),
            new Emoji(char.ConvertFromUtf32(0x1F984), "Unicorn"),
            new Emoji(char.ConvertFromUtf32(0x1F985), "Eagle"),
            new Emoji(char.ConvertFromUtf32(0x1F986), "Duck"),
            new Emoji(char.ConvertFromUtf32(0x1F987), "Bat"),
            new Emoji(char.ConvertFromUtf32(0x1F988), "Shark"),
            new Emoji(char.ConvertFromUtf32(0x1F989), "Owl"),
            new Emoji(char.ConvertFromUtf32(0x1F98A), "Fox"),
            new Emoji(char.ConvertFromUtf32(0x1F98B), "Butterfly"),
            new Emoji(char.ConvertFromUtf32(0x1F98C), "Deer"),
            new Emoji(char.ConvertFromUtf32(0x1F98D), "Gorilla"),
            new Emoji(char.ConvertFromUtf32(0x1F98E), "Lizard"),
            new Emoji(char.ConvertFromUtf32(0x1F98F), "Rhinoceros"),
            new Emoji(char.ConvertFromUtf32(0x1F992), "Giraffe"),
            new Emoji(char.ConvertFromUtf32(0x1F993), "Zebra"),
            new Emoji(char.ConvertFromUtf32(0x1F994), "Hedgehog"),
            new Emoji(char.ConvertFromUtf32(0x1F995), "Sauropod"),
            new Emoji(char.ConvertFromUtf32(0x1F996), "T-Rex"),
            new Emoji(char.ConvertFromUtf32(0x1F997), "Cricket"),
            new Emoji(char.ConvertFromUtf32(0x1F998), "Kangaroo"),
            new Emoji(char.ConvertFromUtf32(0x1F999), "Llama"),
            new Emoji(char.ConvertFromUtf32(0x1F99A), "Peacock"),
            new Emoji(char.ConvertFromUtf32(0x1F99B), "Hippopotamus"),
            new Emoji(char.ConvertFromUtf32(0x1F99C), "Parrot"),
            new Emoji(char.ConvertFromUtf32(0x1F99D), "Raccoon"),
            new Emoji(char.ConvertFromUtf32(0x1F99F), "Mosquito"),
            new Emoji(char.ConvertFromUtf32(0x1F9A0), "Microbe"),
            new Emoji(char.ConvertFromUtf32(0x1F9A1), "Badger"),
            new Emoji(char.ConvertFromUtf32(0x1F9A2), "Swan"),
            new Emoji(char.ConvertFromUtf32(0x1F9A3), "Mammoth"),
            new Emoji(char.ConvertFromUtf32(0x1F9A4), "Dodo"),
            new Emoji(char.ConvertFromUtf32(0x1F9A5), "Sloth"),
            new Emoji(char.ConvertFromUtf32(0x1F9A6), "Otter"),
            new Emoji(char.ConvertFromUtf32(0x1F9A7), "Orangutan"),
            new Emoji(char.ConvertFromUtf32(0x1F9A8), "Skunk"),
            new Emoji(char.ConvertFromUtf32(0x1F9A9), "Flamingo"),
            new Emoji(char.ConvertFromUtf32(0x1F9AB), "Beaver"),
            new Emoji(char.ConvertFromUtf32(0x1F9AC), "Bison"),
            new Emoji(char.ConvertFromUtf32(0x1F9AD), "Seal"),
            new Emoji(char.ConvertFromUtf32(0x1FAB0), "Fly"),
            new Emoji(char.ConvertFromUtf32(0x1FAB1), "Worm"),
            new Emoji(char.ConvertFromUtf32(0x1FAB2), "Beetle"),
            new Emoji(char.ConvertFromUtf32(0x1FAB3), "Cockroach"),
            new Emoji(char.ConvertFromUtf32(0x1FAB6), "Feather"),
            new Emoji(char.ConvertFromUtf32(0x1F9AE), "Guide Dog"));

        public static readonly EmojiGroup nations = new EmojiGroup(
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

        public static readonly EmojiGroup allIcons = new EmojiGroup(
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
