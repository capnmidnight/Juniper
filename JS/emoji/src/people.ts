import { EmojiGroup } from "./EmojiGroup";
import { female, male, transgenderSymbol, lightSkinTone, mediumLightSkinTone, mediumSkinTone, mediumDarkSkinTone, darkSkinTone, redHair, curlyHair, whiteHair, bald, frowning, frowningLightSkinTone, frowningMediumLightSkinTone, frowningMediumSkinTone, frowningMediumDarkSkinTone, frowningDarkSkinTone, frowningMale, frowningLightSkinToneMale, frowningMediumLightSkinToneMale, frowningMediumSkinToneMale, frowningMediumDarkSkinToneMale, frowningDarkSkinToneMale, frowningFemale, frowningLightSkinToneFemale, frowningMediumLightSkinToneFemale, frowningMediumSkinToneFemale, frowningMediumDarkSkinToneFemale, frowningDarkSkinToneFemale, pouting, poutingLightSkinTone, poutingMediumLightSkinTone, poutingMediumSkinTone, poutingMediumDarkSkinTone, poutingDarkSkinTone, poutingMale, poutingLightSkinToneMale, poutingMediumLightSkinToneMale, poutingMediumSkinToneMale, poutingMediumDarkSkinToneMale, poutingDarkSkinToneMale, poutingFemale, poutingLightSkinToneFemale, poutingMediumLightSkinToneFemale, poutingMediumSkinToneFemale, poutingMediumDarkSkinToneFemale, poutingDarkSkinToneFemale, gesturingNO, gesturingNOLightSkinTone, gesturingNOMediumLightSkinTone, gesturingNOMediumSkinTone, gesturingNOMediumDarkSkinTone, gesturingNODarkSkinTone, gesturingNOMale, gesturingNOLightSkinToneMale, gesturingNOMediumLightSkinToneMale, gesturingNOMediumSkinToneMale, gesturingNOMediumDarkSkinToneMale, gesturingNODarkSkinToneMale, gesturingNOFemale, gesturingNOLightSkinToneFemale, gesturingNOMediumLightSkinToneFemale, gesturingNOMediumSkinToneFemale, gesturingNOMediumDarkSkinToneFemale, gesturingNODarkSkinToneFemale, gesturingOK, gesturingOKLightSkinTone, gesturingOKMediumLightSkinTone, gesturingOKMediumSkinTone, gesturingOKMediumDarkSkinTone, gesturingOKDarkSkinTone, gesturingOKMale, gesturingOKLightSkinToneMale, gesturingOKMediumLightSkinToneMale, gesturingOKMediumSkinToneMale, gesturingOKMediumDarkSkinToneMale, gesturingOKDarkSkinToneMale, gesturingOKFemale, gesturingOKLightSkinToneFemale, gesturingOKMediumLightSkinToneFemale, gesturingOKMediumSkinToneFemale, gesturingOKMediumDarkSkinToneFemale, gesturingOKDarkSkinToneFemale, tippingHand, tippingHandLightSkinTone, tippingHandMediumLightSkinTone, tippingHandMediumSkinTone, tippingHandMediumDarkSkinTone, tippingHandDarkSkinTone, tippingHandMale, tippingHandLightSkinToneMale, tippingHandMediumLightSkinToneMale, tippingHandMediumSkinToneMale, tippingHandMediumDarkSkinToneMale, tippingHandDarkSkinToneMale, tippingHandFemale, tippingHandLightSkinToneFemale, tippingHandMediumLightSkinToneFemale, tippingHandMediumSkinToneFemale, tippingHandMediumDarkSkinToneFemale, tippingHandDarkSkinToneFemale, raisingHand, raisingHandLightSkinTone, raisingHandMediumLightSkinTone, raisingHandMediumSkinTone, raisingHandMediumDarkSkinTone, raisingHandDarkSkinTone, raisingHandMale, raisingHandLightSkinToneMale, raisingHandMediumLightSkinToneMale, raisingHandMediumSkinToneMale, raisingHandMediumDarkSkinToneMale, raisingHandDarkSkinToneMale, raisingHandFemale, raisingHandLightSkinToneFemale, raisingHandMediumLightSkinToneFemale, raisingHandMediumSkinToneFemale, raisingHandMediumDarkSkinToneFemale, raisingHandDarkSkinToneFemale, bowing, bowingLightSkinTone, bowingMediumLightSkinTone, bowingMediumSkinTone, bowingMediumDarkSkinTone, bowingDarkSkinTone, bowingMale, bowingLightSkinToneMale, bowingMediumLightSkinToneMale, bowingMediumSkinToneMale, bowingMediumDarkSkinToneMale, bowingDarkSkinToneMale, bowingFemale, bowingLightSkinToneFemale, bowingMediumLightSkinToneFemale, bowingMediumSkinToneFemale, bowingMediumDarkSkinToneFemale, bowingDarkSkinToneFemale, facepalming, facepalmingLightSkinTone, facepalmingMediumLightSkinTone, facepalmingMediumSkinTone, facepalmingMediumDarkSkinTone, facepalmingDarkSkinTone, facepalmingMale, facepalmingLightSkinToneMale, facepalmingMediumLightSkinToneMale, facepalmingMediumSkinToneMale, facepalmingMediumDarkSkinToneMale, facepalmingDarkSkinToneMale, facepalmingFemale, facepalmingLightSkinToneFemale, facepalmingMediumLightSkinToneFemale, facepalmingMediumSkinToneFemale, facepalmingMediumDarkSkinToneFemale, facepalmingDarkSkinToneFemale, shrugging, shruggingLightSkinTone, shruggingMediumLightSkinTone, shruggingMediumSkinTone, shruggingMediumDarkSkinTone, shruggingDarkSkinTone, shruggingMale, shruggingLightSkinToneMale, shruggingMediumLightSkinToneMale, shruggingMediumSkinToneMale, shruggingMediumDarkSkinToneMale, shruggingDarkSkinToneMale, shruggingFemale, shruggingLightSkinToneFemale, shruggingMediumLightSkinToneFemale, shruggingMediumSkinToneFemale, shruggingMediumDarkSkinToneFemale, shruggingDarkSkinToneFemale, cantHear, cantHearLightSkinTone, cantHearMediumLightSkinTone, cantHearMediumSkinTone, cantHearMediumDarkSkinTone, cantHearDarkSkinTone, cantHearMale, cantHearLightSkinToneMale, cantHearMediumLightSkinToneMale, cantHearMediumSkinToneMale, cantHearMediumDarkSkinToneMale, cantHearDarkSkinToneMale, cantHearFemale, cantHearLightSkinToneFemale, cantHearMediumLightSkinToneFemale, cantHearMediumSkinToneFemale, cantHearMediumDarkSkinToneFemale, cantHearDarkSkinToneFemale, gettingMassage, gettingMassageLightSkinTone, gettingMassageMediumLightSkinTone, gettingMassageMediumSkinTone, gettingMassageMediumDarkSkinTone, gettingMassageDarkSkinTone, gettingMassageMale, gettingMassageLightSkinToneMale, gettingMassageMediumLightSkinToneMale, gettingMassageMediumSkinToneMale, gettingMassageMediumDarkSkinToneMale, gettingMassageDarkSkinToneMale, gettingMassageFemale, gettingMassageLightSkinToneFemale, gettingMassageMediumLightSkinToneFemale, gettingMassageMediumSkinToneFemale, gettingMassageMediumDarkSkinToneFemale, gettingMassageDarkSkinToneFemale, gettingHaircut, gettingHaircutLightSkinTone, gettingHaircutMediumLightSkinTone, gettingHaircutMediumSkinTone, gettingHaircutMediumDarkSkinTone, gettingHaircutDarkSkinTone, gettingHaircutMale, gettingHaircutLightSkinToneMale, gettingHaircutMediumLightSkinToneMale, gettingHaircutMediumSkinToneMale, gettingHaircutMediumDarkSkinToneMale, gettingHaircutDarkSkinToneMale, gettingHaircutFemale, gettingHaircutLightSkinToneFemale, gettingHaircutMediumLightSkinToneFemale, gettingHaircutMediumSkinToneFemale, gettingHaircutMediumDarkSkinToneFemale, gettingHaircutDarkSkinToneFemale, constructionWorker, constructionWorkerLightSkinTone, constructionWorkerMediumLightSkinTone, constructionWorkerMediumSkinTone, constructionWorkerMediumDarkSkinTone, constructionWorkerDarkSkinTone, constructionWorkerMale, constructionWorkerLightSkinToneMale, constructionWorkerMediumLightSkinToneMale, constructionWorkerMediumSkinToneMale, constructionWorkerMediumDarkSkinToneMale, constructionWorkerDarkSkinToneMale, constructionWorkerFemale, constructionWorkerLightSkinToneFemale, constructionWorkerMediumLightSkinToneFemale, constructionWorkerMediumSkinToneFemale, constructionWorkerMediumDarkSkinToneFemale, constructionWorkerDarkSkinToneFemale, guard, guardLightSkinTone, guardMediumLightSkinTone, guardMediumSkinTone, guardMediumDarkSkinTone, guardDarkSkinTone, guardMale, guardLightSkinToneMale, guardMediumLightSkinToneMale, guardMediumSkinToneMale, guardMediumDarkSkinToneMale, guardDarkSkinToneMale, guardFemale, guardLightSkinToneFemale, guardMediumLightSkinToneFemale, guardMediumSkinToneFemale, guardMediumDarkSkinToneFemale, guardDarkSkinToneFemale, spy, spyLightSkinTone, spyMediumLightSkinTone, spyMediumSkinTone, spyMediumDarkSkinTone, spyDarkSkinTone, spyMale, spyLightSkinToneMale, spyMediumLightSkinToneMale, spyMediumSkinToneMale, spyMediumDarkSkinToneMale, spyDarkSkinToneMale, spyFemale, spyLightSkinToneFemale, spyMediumLightSkinToneFemale, spyMediumSkinToneFemale, spyMediumDarkSkinToneFemale, spyDarkSkinToneFemale, police, policeLightSkinTone, policeMediumLightSkinTone, policeMediumSkinTone, policeMediumDarkSkinTone, policeDarkSkinTone, policeMale, policeLightSkinToneMale, policeMediumLightSkinToneMale, policeMediumSkinToneMale, policeMediumDarkSkinToneMale, policeDarkSkinToneMale, policeFemale, policeLightSkinToneFemale, policeMediumLightSkinToneFemale, policeMediumSkinToneFemale, policeMediumDarkSkinToneFemale, policeDarkSkinToneFemale, wearingTurban, wearingTurbanLightSkinTone, wearingTurbanMediumLightSkinTone, wearingTurbanMediumSkinTone, wearingTurbanMediumDarkSkinTone, wearingTurbanDarkSkinTone, wearingTurbanMale, wearingTurbanLightSkinToneMale, wearingTurbanMediumLightSkinToneMale, wearingTurbanMediumSkinToneMale, wearingTurbanMediumDarkSkinToneMale, wearingTurbanDarkSkinToneMale, wearingTurbanFemale, wearingTurbanLightSkinToneFemale, wearingTurbanMediumLightSkinToneFemale, wearingTurbanMediumSkinToneFemale, wearingTurbanMediumDarkSkinToneFemale, wearingTurbanDarkSkinToneFemale, superhero, superheroLightSkinTone, superheroMediumLightSkinTone, superheroMediumSkinTone, superheroMediumDarkSkinTone, superheroDarkSkinTone, superheroMale, superheroLightSkinToneMale, superheroMediumLightSkinToneMale, superheroMediumSkinToneMale, superheroMediumDarkSkinToneMale, superheroDarkSkinToneMale, superheroFemale, superheroLightSkinToneFemale, superheroMediumLightSkinToneFemale, superheroMediumSkinToneFemale, superheroMediumDarkSkinToneFemale, superheroDarkSkinToneFemale, supervillain, supervillainLightSkinTone, supervillainMediumLightSkinTone, supervillainMediumSkinTone, supervillainMediumDarkSkinTone, supervillainDarkSkinTone, supervillainMale, supervillainLightSkinToneMale, supervillainMediumLightSkinToneMale, supervillainMediumSkinToneMale, supervillainMediumDarkSkinToneMale, supervillainDarkSkinToneMale, supervillainFemale, supervillainLightSkinToneFemale, supervillainMediumLightSkinToneFemale, supervillainMediumSkinToneFemale, supervillainMediumDarkSkinToneFemale, supervillainDarkSkinToneFemale, mage, mageLightSkinTone, mageMediumLightSkinTone, mageMediumSkinTone, mageMediumDarkSkinTone, mageDarkSkinTone, mageMale, mageLightSkinToneMale, mageMediumLightSkinToneMale, mageMediumSkinToneMale, mageMediumDarkSkinToneMale, mageDarkSkinToneMale, mageFemale, mageLightSkinToneFemale, mageMediumLightSkinToneFemale, mageMediumSkinToneFemale, mageMediumDarkSkinToneFemale, mageDarkSkinToneFemale, fairy, fairyLightSkinTone, fairyMediumLightSkinTone, fairyMediumSkinTone, fairyMediumDarkSkinTone, fairyDarkSkinTone, fairyMale, fairyLightSkinToneMale, fairyMediumLightSkinToneMale, fairyMediumSkinToneMale, fairyMediumDarkSkinToneMale, fairyDarkSkinToneMale, fairyFemale, fairyLightSkinToneFemale, fairyMediumLightSkinToneFemale, fairyMediumSkinToneFemale, fairyMediumDarkSkinToneFemale, fairyDarkSkinToneFemale, vampire, vampireLightSkinTone, vampireMediumLightSkinTone, vampireMediumSkinTone, vampireMediumDarkSkinTone, vampireDarkSkinTone, vampireMale, vampireLightSkinToneMale, vampireMediumLightSkinToneMale, vampireMediumSkinToneMale, vampireMediumDarkSkinToneMale, vampireDarkSkinToneMale, vampireFemale, vampireLightSkinToneFemale, vampireMediumLightSkinToneFemale, vampireMediumSkinToneFemale, vampireMediumDarkSkinToneFemale, vampireDarkSkinToneFemale, merperson, merpersonLightSkinTone, merpersonMediumLightSkinTone, merpersonMediumSkinTone, merpersonMediumDarkSkinTone, merpersonDarkSkinTone, merpersonMale, merpersonLightSkinToneMale, merpersonMediumLightSkinToneMale, merpersonMediumSkinToneMale, merpersonMediumDarkSkinToneMale, merpersonDarkSkinToneMale, merpersonFemale, merpersonLightSkinToneFemale, merpersonMediumLightSkinToneFemale, merpersonMediumSkinToneFemale, merpersonMediumDarkSkinToneFemale, merpersonDarkSkinToneFemale, elf, elfLightSkinTone, elfMediumLightSkinTone, elfMediumSkinTone, elfMediumDarkSkinTone, elfDarkSkinTone, elfMale, elfLightSkinToneMale, elfMediumLightSkinToneMale, elfMediumSkinToneMale, elfMediumDarkSkinToneMale, elfDarkSkinToneMale, elfFemale, elfLightSkinToneFemale, elfMediumLightSkinToneFemale, elfMediumSkinToneFemale, elfMediumDarkSkinToneFemale, elfDarkSkinToneFemale, walking, walkingLightSkinTone, walkingMediumLightSkinTone, walkingMediumSkinTone, walkingMediumDarkSkinTone, walkingDarkSkinTone, walkingMale, walkingLightSkinToneMale, walkingMediumLightSkinToneMale, walkingMediumSkinToneMale, walkingMediumDarkSkinToneMale, walkingDarkSkinToneMale, walkingFemale, walkingLightSkinToneFemale, walkingMediumLightSkinToneFemale, walkingMediumSkinToneFemale, walkingMediumDarkSkinToneFemale, walkingDarkSkinToneFemale, standing, standingLightSkinTone, standingMediumLightSkinTone, standingMediumSkinTone, standingMediumDarkSkinTone, standingDarkSkinTone, standingMale, standingLightSkinToneMale, standingMediumLightSkinToneMale, standingMediumSkinToneMale, standingMediumDarkSkinToneMale, standingDarkSkinToneMale, standingFemale, standingLightSkinToneFemale, standingMediumLightSkinToneFemale, standingMediumSkinToneFemale, standingMediumDarkSkinToneFemale, standingDarkSkinToneFemale, kneeling, kneelingLightSkinTone, kneelingMediumLightSkinTone, kneelingMediumSkinTone, kneelingMediumDarkSkinTone, kneelingDarkSkinTone, kneelingMale, kneelingLightSkinToneMale, kneelingMediumLightSkinToneMale, kneelingMediumSkinToneMale, kneelingMediumDarkSkinToneMale, kneelingDarkSkinToneMale, kneelingFemale, kneelingLightSkinToneFemale, kneelingMediumLightSkinToneFemale, kneelingMediumSkinToneFemale, kneelingMediumDarkSkinToneFemale, kneelingDarkSkinToneFemale, running, runningLightSkinTone, runningMediumLightSkinTone, runningMediumSkinTone, runningMediumDarkSkinTone, runningDarkSkinTone, runningMale, runningLightSkinToneMale, runningMediumLightSkinToneMale, runningMediumSkinToneMale, runningMediumDarkSkinToneMale, runningDarkSkinToneMale, runningFemale, runningLightSkinToneFemale, runningMediumLightSkinToneFemale, runningMediumSkinToneFemale, runningMediumDarkSkinToneFemale, runningDarkSkinToneFemale, baby, babyLightSkinTone, babyMediumLightSkinTone, babyMediumSkinTone, babyMediumDarkSkinTone, babyDarkSkinTone, child, childLightSkinTone, childMediumLightSkinTone, childMediumSkinTone, childMediumDarkSkinTone, childDarkSkinTone, boy, boyLightSkinTone, boyMediumLightSkinTone, boyMediumSkinTone, boyMediumDarkSkinTone, boyDarkSkinTone, girl, girlLightSkinTone, girlMediumLightSkinTone, girlMediumSkinTone, girlMediumDarkSkinTone, girlDarkSkinTone, blondPerson, blondPersonLightSkinTone, blondPersonMediumLightSkinTone, blondPersonMediumSkinTone, blondPersonMediumDarkSkinTone, blondPersonDarkSkinTone, blondPersonMale, blondPersonLightSkinToneMale, blondPersonMediumLightSkinToneMale, blondPersonMediumSkinToneMale, blondPersonMediumDarkSkinToneMale, blondPersonDarkSkinToneMale, blondPersonFemale, blondPersonLightSkinToneFemale, blondPersonMediumLightSkinToneFemale, blondPersonMediumSkinToneFemale, blondPersonMediumDarkSkinToneFemale, blondPersonDarkSkinToneFemale, person, personLightSkinTone, personMediumLightSkinTone, personMediumSkinTone, personMediumDarkSkinTone, personDarkSkinTone, beardedMan, beardedManLightSkinTone, beardedManMediumLightSkinTone, beardedManMediumSkinTone, beardedManMediumDarkSkinTone, beardedManDarkSkinTone, manWithChineseCap, manWithChineseCapLightSkinTone, manWithChineseCapMediumLightSkinTone, manWithChineseCapMediumSkinTone, manWithChineseCapMediumDarkSkinTone, manWithChineseCapDarkSkinTone, manInTuxedo, manInTuxedoLightSkinTone, manInTuxedoMediumLightSkinTone, manInTuxedoMediumSkinTone, manInTuxedoMediumDarkSkinTone, manInTuxedoDarkSkinTone, man, manLightSkinTone, manMediumLightSkinTone, manMediumSkinTone, manMediumDarkSkinTone, manDarkSkinTone, manRedHair, manLightSkinToneRedHair, manMediumLightSkinToneRedHair, manMediumSkinToneRedHair, manMediumDarkSkinToneRedHair, manDarkSkinToneRedHair, manCurlyHair, manLightSkinToneCurlyHair, manMediumLightSkinToneCurlyHair, manMediumSkinToneCurlyHair, manMediumDarkSkinToneCurlyHair, manDarkSkinToneCurlyHair, manWhiteHair, manLightSkinToneWhiteHair, manMediumLightSkinToneWhiteHair, manMediumSkinToneWhiteHair, manMediumDarkSkinToneWhiteHair, manDarkSkinToneWhiteHair, manBald, manLightSkinToneBald, manMediumLightSkinToneBald, manMediumSkinToneBald, manMediumDarkSkinToneBald, manDarkSkinToneBald, manInSuitLevitating, pregnantWoman, pregnantWomanLightSkinTone, pregnantWomanMediumLightSkinTone, pregnantWomanMediumSkinTone, pregnantWomanMediumDarkSkinTone, pregnantWomanDarkSkinTone, breastFeeding, breastFeedingLightSkinTone, breastFeedingMediumLightSkinTone, breastFeedingMediumSkinTone, breastFeedingMediumDarkSkinTone, breastFeedingDarkSkinTone, womanWithHeadscarf, womanWithHeadscarfLightSkinTone, womanWithHeadscarfMediumLightSkinTone, womanWithHeadscarfMediumSkinTone, womanWithHeadscarfMediumDarkSkinTone, womanWithHeadscarfDarkSkinTone, brideWithVeil, brideWithVeilLightSkinTone, brideWithVeilMediumLightSkinTone, brideWithVeilMediumSkinTone, brideWithVeilMediumDarkSkinTone, brideWithVeilDarkSkinTone, woman, womanLightSkinTone, womanMediumLightSkinTone, womanMediumSkinTone, womanMediumDarkSkinTone, womanDarkSkinTone, womanRedHair, womanLightSkinToneRedHair, womanMediumLightSkinToneRedHair, womanMediumSkinToneRedHair, womanMediumDarkSkinToneRedHair, womanDarkSkinToneRedHair, womanCurlyHair, womanLightSkinToneCurlyHair, womanMediumLightSkinToneCurlyHair, womanMediumSkinToneCurlyHair, womanMediumDarkSkinToneCurlyHair, womanDarkSkinToneCurlyHair, womanWhiteHair, womanLightSkinToneWhiteHair, womanMediumLightSkinToneWhiteHair, womanMediumSkinToneWhiteHair, womanMediumDarkSkinToneWhiteHair, womanDarkSkinToneWhiteHair, womanBald, womanLightSkinToneBald, womanMediumLightSkinToneBald, womanMediumSkinToneBald, womanMediumDarkSkinToneBald, womanDarkSkinToneBald, olderPerson, olderPersonLightSkinTone, olderPersonMediumLightSkinTone, olderPersonMediumSkinTone, olderPersonMediumDarkSkinTone, olderPersonDarkSkinTone, oldMan, oldManLightSkinTone, oldManMediumLightSkinTone, oldManMediumSkinTone, oldManMediumDarkSkinTone, oldManDarkSkinTone, oldWoman, oldWomanLightSkinTone, oldWomanMediumLightSkinTone, oldWomanMediumSkinTone, oldWomanMediumDarkSkinTone, oldWomanDarkSkinTone, manHealthCare, manLightSkinToneHealthCare, manMediumLightSkinToneHealthCare, manMediumSkinToneHealthCare, manMediumDarkSkinToneHealthCare, manDarkSkinToneHealthCare, womanHealthCare, womanLightSkinToneHealthCare, womanMediumLightSkinToneHealthCare, womanMediumSkinToneHealthCare, womanMediumDarkSkinToneHealthCare, womanDarkSkinToneHealthCare, medical, manStudent, manLightSkinToneStudent, manMediumLightSkinToneStudent, manMediumSkinToneStudent, manMediumDarkSkinToneStudent, manDarkSkinToneStudent, womanStudent, womanLightSkinToneStudent, womanMediumLightSkinToneStudent, womanMediumSkinToneStudent, womanMediumDarkSkinToneStudent, womanDarkSkinToneStudent, graduationCap, manTeacher, manLightSkinToneTeacher, manMediumLightSkinToneTeacher, manMediumSkinToneTeacher, manMediumDarkSkinToneTeacher, manDarkSkinToneTeacher, womanTeacher, womanLightSkinToneTeacher, womanMediumLightSkinToneTeacher, womanMediumSkinToneTeacher, womanMediumDarkSkinToneTeacher, womanDarkSkinToneTeacher, school, manJudge, manLightSkinToneJudge, manMediumLightSkinToneJudge, manMediumSkinToneJudge, manMediumDarkSkinToneJudge, manDarkSkinToneJudge, womanJudge, womanLightSkinToneJudge, womanMediumLightSkinToneJudge, womanMediumSkinToneJudge, womanMediumDarkSkinToneJudge, womanDarkSkinToneJudge, balanceScale, manFarmer, manLightSkinToneFarmer, manMediumLightSkinToneFarmer, manMediumSkinToneFarmer, manMediumDarkSkinToneFarmer, manDarkSkinToneFarmer, womanFarmer, womanLightSkinToneFarmer, womanMediumLightSkinToneFarmer, womanMediumSkinToneFarmer, womanMediumDarkSkinToneFarmer, womanDarkSkinToneFarmer, sheafOfRice, manCook, manLightSkinToneCook, manMediumLightSkinToneCook, manMediumSkinToneCook, manMediumDarkSkinToneCook, manDarkSkinToneCook, womanCook, womanLightSkinToneCook, womanMediumLightSkinToneCook, womanMediumSkinToneCook, womanMediumDarkSkinToneCook, womanDarkSkinToneCook, cooking, manMechanic, manLightSkinToneMechanic, manMediumLightSkinToneMechanic, manMediumSkinToneMechanic, manMediumDarkSkinToneMechanic, manDarkSkinToneMechanic, womanMechanic, womanLightSkinToneMechanic, womanMediumLightSkinToneMechanic, womanMediumSkinToneMechanic, womanMediumDarkSkinToneMechanic, womanDarkSkinToneMechanic, wrench, manFactoryWorker, manLightSkinToneFactoryWorker, manMediumLightSkinToneFactoryWorker, manMediumSkinToneFactoryWorker, manMediumDarkSkinToneFactoryWorker, manDarkSkinToneFactoryWorker, womanFactoryWorker, womanLightSkinToneFactoryWorker, womanMediumLightSkinToneFactoryWorker, womanMediumSkinToneFactoryWorker, womanMediumDarkSkinToneFactoryWorker, womanDarkSkinToneFactoryWorker, factory, manOfficeWorker, manLightSkinToneOfficeWorker, manMediumLightSkinToneOfficeWorker, manMediumSkinToneOfficeWorker, manMediumDarkSkinToneOfficeWorker, manDarkSkinToneOfficeWorker, womanOfficeWorker, womanLightSkinToneOfficeWorker, womanMediumLightSkinToneOfficeWorker, womanMediumSkinToneOfficeWorker, womanMediumDarkSkinToneOfficeWorker, womanDarkSkinToneOfficeWorker, briefcase, manFireFighter, manLightSkinToneFireFighter, manMediumLightSkinToneFireFighter, manMediumSkinToneFireFighter, manMediumDarkSkinToneFireFighter, manDarkSkinToneFireFighter, womanFireFighter, womanLightSkinToneFireFighter, womanMediumLightSkinToneFireFighter, womanMediumSkinToneFireFighter, womanMediumDarkSkinToneFireFighter, womanDarkSkinToneFireFighter, fireEngine, manAstronaut, manLightSkinToneAstronaut, manMediumLightSkinToneAstronaut, manMediumSkinToneAstronaut, manMediumDarkSkinToneAstronaut, manDarkSkinToneAstronaut, womanAstronaut, womanLightSkinToneAstronaut, womanMediumLightSkinToneAstronaut, womanMediumSkinToneAstronaut, womanMediumDarkSkinToneAstronaut, womanDarkSkinToneAstronaut, rocket, manPilot, manLightSkinTonePilot, manMediumLightSkinTonePilot, manMediumSkinTonePilot, manMediumDarkSkinTonePilot, manDarkSkinTonePilot, womanPilot, womanLightSkinTonePilot, womanMediumLightSkinTonePilot, womanMediumSkinTonePilot, womanMediumDarkSkinTonePilot, womanDarkSkinTonePilot, airplane, manArtist, manLightSkinToneArtist, manMediumLightSkinToneArtist, manMediumSkinToneArtist, manMediumDarkSkinToneArtist, manDarkSkinToneArtist, womanArtist, womanLightSkinToneArtist, womanMediumLightSkinToneArtist, womanMediumSkinToneArtist, womanMediumDarkSkinToneArtist, womanDarkSkinToneArtist, artistPalette, manSinger, manLightSkinToneSinger, manMediumLightSkinToneSinger, manMediumSkinToneSinger, manMediumDarkSkinToneSinger, manDarkSkinToneSinger, womanSinger, womanLightSkinToneSinger, womanMediumLightSkinToneSinger, womanMediumSkinToneSinger, womanMediumDarkSkinToneSinger, womanDarkSkinToneSinger, microphone, manTechnologist, manLightSkinToneTechnologist, manMediumLightSkinToneTechnologist, manMediumSkinToneTechnologist, manMediumDarkSkinToneTechnologist, manDarkSkinToneTechnologist, womanTechnologist, womanLightSkinToneTechnologist, womanMediumLightSkinToneTechnologist, womanMediumSkinToneTechnologist, womanMediumDarkSkinToneTechnologist, womanDarkSkinToneTechnologist, laptop, manScientist, manLightSkinToneScientist, manMediumLightSkinToneScientist, manMediumSkinToneScientist, manMediumDarkSkinToneScientist, manDarkSkinToneScientist, womanScientist, womanLightSkinToneScientist, womanMediumLightSkinToneScientist, womanMediumSkinToneScientist, womanMediumDarkSkinToneScientist, womanDarkSkinToneScientist, microscope, prince, princeLightSkinTone, princeMediumLightSkinTone, princeMediumSkinTone, princeMediumDarkSkinTone, princeDarkSkinTone, princess, princessLightSkinTone, princessMediumLightSkinTone, princessMediumSkinTone, princessMediumDarkSkinTone, princessDarkSkinTone, crown, cherub, cherubLightSkinTone, cherubMediumLightSkinTone, cherubMediumSkinTone, cherubMediumDarkSkinTone, cherubDarkSkinTone, santaClaus, santaClausLightSkinTone, santaClausMediumLightSkinTone, santaClausMediumSkinTone, santaClausMediumDarkSkinTone, santaClausDarkSkinTone, mrsClaus, mrsClausLightSkinTone, mrsClausMediumLightSkinTone, mrsClausMediumSkinTone, mrsClausMediumDarkSkinTone, mrsClausDarkSkinTone, genie, genieMale, genieFemale, zombie, zombieMale, zombieFemale, manProbing, manLightSkinToneProbing, manMediumLightSkinToneProbing, manMediumSkinToneProbing, manMediumDarkSkinToneProbing, manDarkSkinToneProbing, womanProbing, womanLightSkinToneProbing, womanMediumLightSkinToneProbing, womanMediumSkinToneProbing, womanMediumDarkSkinToneProbing, womanDarkSkinToneProbing, probingCane, manInMotorizedWheelchair, manLightSkinToneInMotorizedWheelchair, manMediumLightSkinToneInMotorizedWheelchair, manMediumSkinToneInMotorizedWheelchair, manMediumDarkSkinToneInMotorizedWheelchair, manDarkSkinToneInMotorizedWheelchair, womanInMotorizedWheelchair, womanLightSkinToneInMotorizedWheelchair, womanMediumLightSkinToneInMotorizedWheelchair, womanMediumSkinToneInMotorizedWheelchair, womanMediumDarkSkinToneInMotorizedWheelchair, womanDarkSkinToneInMotorizedWheelchair, motorizedWheelchair, manInManualWheelchair, manLightSkinToneInManualWheelchair, manMediumLightSkinToneInManualWheelchair, manMediumSkinToneInManualWheelchair, manMediumDarkSkinToneInManualWheelchair, manDarkSkinToneInManualWheelchair, womanInManualWheelchair, womanLightSkinToneInManualWheelchair, womanMediumLightSkinToneInManualWheelchair, womanMediumSkinToneInManualWheelchair, womanMediumDarkSkinToneInManualWheelchair, womanDarkSkinToneInManualWheelchair, manualWheelchair, manDancing, manDancingLightSkinTone, manDancingMediumLightSkinTone, manDancingMediumSkinTone, manDancingMediumDarkSkinTone, manDancingDarkSkinTone, womanDancing, womanDancingLightSkinTone, womanDancingMediumLightSkinTone, womanDancingMediumSkinTone, womanDancingMediumDarkSkinTone, womanDancingDarkSkinTone, juggler, jugglerLightSkinTone, jugglerMediumLightSkinTone, jugglerMediumSkinTone, jugglerMediumDarkSkinTone, jugglerDarkSkinTone, jugglerMale, jugglerLightSkinToneMale, jugglerMediumLightSkinToneMale, jugglerMediumSkinToneMale, jugglerMediumDarkSkinToneMale, jugglerDarkSkinToneMale, jugglerFemale, jugglerLightSkinToneFemale, jugglerMediumLightSkinToneFemale, jugglerMediumSkinToneFemale, jugglerMediumDarkSkinToneFemale, jugglerDarkSkinToneFemale, climber, climberLightSkinTone, climberMediumLightSkinTone, climberMediumSkinTone, climberMediumDarkSkinTone, climberDarkSkinTone, climberMale, climberLightSkinToneMale, climberMediumLightSkinToneMale, climberMediumSkinToneMale, climberMediumDarkSkinToneMale, climberDarkSkinToneMale, climberFemale, climberLightSkinToneFemale, climberMediumLightSkinToneFemale, climberMediumSkinToneFemale, climberMediumDarkSkinToneFemale, climberDarkSkinToneFemale, jockey, jockeyLightSkinTone, jockeyMediumLightSkinTone, jockeyMediumSkinTone, jockeyMediumDarkSkinTone, jockeyDarkSkinTone, snowboarder, snowboarderLightSkinTone, snowboarderMediumLightSkinTone, snowboarderMediumSkinTone, snowboarderMediumDarkSkinTone, snowboarderDarkSkinTone, golfer, golferLightSkinTone, golferMediumLightSkinTone, golferMediumSkinTone, golferMediumDarkSkinTone, golferDarkSkinTone, golferMale, golferLightSkinToneMale, golferMediumLightSkinToneMale, golferMediumSkinToneMale, golferMediumDarkSkinToneMale, golferDarkSkinToneMale, golferFemale, golferLightSkinToneFemale, golferMediumLightSkinToneFemale, golferMediumSkinToneFemale, golferMediumDarkSkinToneFemale, golferDarkSkinToneFemale, surfing, surfingLightSkinTone, surfingMediumLightSkinTone, surfingMediumSkinTone, surfingMediumDarkSkinTone, surfingDarkSkinTone, surfingMale, surfingLightSkinToneMale, surfingMediumLightSkinToneMale, surfingMediumSkinToneMale, surfingMediumDarkSkinToneMale, surfingDarkSkinToneMale, surfingFemale, surfingLightSkinToneFemale, surfingMediumLightSkinToneFemale, surfingMediumSkinToneFemale, surfingMediumDarkSkinToneFemale, surfingDarkSkinToneFemale, rowingBoat, rowingBoatLightSkinTone, rowingBoatMediumLightSkinTone, rowingBoatMediumSkinTone, rowingBoatMediumDarkSkinTone, rowingBoatDarkSkinTone, rowingBoatMale, rowingBoatLightSkinToneMale, rowingBoatMediumLightSkinToneMale, rowingBoatMediumSkinToneMale, rowingBoatMediumDarkSkinToneMale, rowingBoatDarkSkinToneMale, rowingBoatFemale, rowingBoatLightSkinToneFemale, rowingBoatMediumLightSkinToneFemale, rowingBoatMediumSkinToneFemale, rowingBoatMediumDarkSkinToneFemale, rowingBoatDarkSkinToneFemale, swimming, swimmingLightSkinTone, swimmingMediumLightSkinTone, swimmingMediumSkinTone, swimmingMediumDarkSkinTone, swimmingDarkSkinTone, swimmingMale, swimmingLightSkinToneMale, swimmingMediumLightSkinToneMale, swimmingMediumSkinToneMale, swimmingMediumDarkSkinToneMale, swimmingDarkSkinToneMale, swimmingFemale, swimmingLightSkinToneFemale, swimmingMediumLightSkinToneFemale, swimmingMediumSkinToneFemale, swimmingMediumDarkSkinToneFemale, swimmingDarkSkinToneFemale, basketBaller, basketBallerLightSkinTone, basketBallerMediumLightSkinTone, basketBallerMediumSkinTone, basketBallerMediumDarkSkinTone, basketBallerDarkSkinTone, basketBallerMale, basketBallerLightSkinToneMale, basketBallerMediumLightSkinToneMale, basketBallerMediumSkinToneMale, basketBallerMediumDarkSkinToneMale, basketBallerDarkSkinToneMale, basketBallerFemale, basketBallerLightSkinToneFemale, basketBallerMediumLightSkinToneFemale, basketBallerMediumSkinToneFemale, basketBallerMediumDarkSkinToneFemale, basketBallerDarkSkinToneFemale, weightLifter, weightLifterLightSkinTone, weightLifterMediumLightSkinTone, weightLifterMediumSkinTone, weightLifterMediumDarkSkinTone, weightLifterDarkSkinTone, weightLifterMale, weightLifterLightSkinToneMale, weightLifterMediumLightSkinToneMale, weightLifterMediumSkinToneMale, weightLifterMediumDarkSkinToneMale, weightLifterDarkSkinToneMale, weightLifterFemale, weightLifterLightSkinToneFemale, weightLifterMediumLightSkinToneFemale, weightLifterMediumSkinToneFemale, weightLifterMediumDarkSkinToneFemale, weightLifterDarkSkinToneFemale, biker, bikerLightSkinTone, bikerMediumLightSkinTone, bikerMediumSkinTone, bikerMediumDarkSkinTone, bikerDarkSkinTone, bikerMale, bikerLightSkinToneMale, bikerMediumLightSkinToneMale, bikerMediumSkinToneMale, bikerMediumDarkSkinToneMale, bikerDarkSkinToneMale, bikerFemale, bikerLightSkinToneFemale, bikerMediumLightSkinToneFemale, bikerMediumSkinToneFemale, bikerMediumDarkSkinToneFemale, bikerDarkSkinToneFemale, mountainBiker, mountainBikerLightSkinTone, mountainBikerMediumLightSkinTone, mountainBikerMediumSkinTone, mountainBikerMediumDarkSkinTone, mountainBikerDarkSkinTone, mountainBikerMale, mountainBikerLightSkinToneMale, mountainBikerMediumLightSkinToneMale, mountainBikerMediumSkinToneMale, mountainBikerMediumDarkSkinToneMale, mountainBikerDarkSkinToneMale, mountainBikerFemale, mountainBikerLightSkinToneFemale, mountainBikerMediumLightSkinToneFemale, mountainBikerMediumSkinToneFemale, mountainBikerMediumDarkSkinToneFemale, mountainBikerDarkSkinToneFemale, cartwheeler, cartwheelerLightSkinTone, cartwheelerMediumLightSkinTone, cartwheelerMediumSkinTone, cartwheelerMediumDarkSkinTone, cartwheelerDarkSkinTone, cartwheelerMale, cartwheelerLightSkinToneMale, cartwheelerMediumLightSkinToneMale, cartwheelerMediumSkinToneMale, cartwheelerMediumDarkSkinToneMale, cartwheelerDarkSkinToneMale, cartwheelerFemale, cartwheelerLightSkinToneFemale, cartwheelerMediumLightSkinToneFemale, cartwheelerMediumSkinToneFemale, cartwheelerMediumDarkSkinToneFemale, cartwheelerDarkSkinToneFemale, wrestler, wrestlerMale, wrestlerFemale, waterPoloPlayer, waterPoloPlayerLightSkinTone, waterPoloPlayerMediumLightSkinTone, waterPoloPlayerMediumSkinTone, waterPoloPlayerMediumDarkSkinTone, waterPoloPlayerDarkSkinTone, waterPoloPlayerMale, waterPoloPlayerLightSkinToneMale, waterPoloPlayerMediumLightSkinToneMale, waterPoloPlayerMediumSkinToneMale, waterPoloPlayerMediumDarkSkinToneMale, waterPoloPlayerDarkSkinToneMale, waterPoloPlayerFemale, waterPoloPlayerLightSkinToneFemale, waterPoloPlayerMediumLightSkinToneFemale, waterPoloPlayerMediumSkinToneFemale, waterPoloPlayerMediumDarkSkinToneFemale, waterPoloPlayerDarkSkinToneFemale, handBaller, handBallerLightSkinTone, handBallerMediumLightSkinTone, handBallerMediumSkinTone, handBallerMediumDarkSkinTone, handBallerDarkSkinTone, handBallerMale, handBallerLightSkinToneMale, handBallerMediumLightSkinToneMale, handBallerMediumSkinToneMale, handBallerMediumDarkSkinToneMale, handBallerDarkSkinToneMale, handBallerFemale, handBallerLightSkinToneFemale, handBallerMediumLightSkinToneFemale, handBallerMediumSkinToneFemale, handBallerMediumDarkSkinToneFemale, handBallerDarkSkinToneFemale, fencer, skier, inLotusPosition, inLotusPositionLightSkinTone, inLotusPositionMediumLightSkinTone, inLotusPositionMediumSkinTone, inLotusPositionMediumDarkSkinTone, inLotusPositionDarkSkinTone, inLotusPositionMale, inLotusPositionLightSkinToneMale, inLotusPositionMediumLightSkinToneMale, inLotusPositionMediumSkinToneMale, inLotusPositionMediumDarkSkinToneMale, inLotusPositionDarkSkinToneMale, inLotusPositionFemale, inLotusPositionLightSkinToneFemale, inLotusPositionMediumLightSkinToneFemale, inLotusPositionMediumSkinToneFemale, inLotusPositionMediumDarkSkinToneFemale, inLotusPositionDarkSkinToneFemale, inBath, inBathLightSkinTone, inBathMediumLightSkinTone, inBathMediumSkinTone, inBathMediumDarkSkinTone, inBathDarkSkinTone, inBed, inBedLightSkinTone, inBedMediumLightSkinTone, inBedMediumSkinTone, inBedMediumDarkSkinTone, inBedDarkSkinTone, inSauna, inSaunaLightSkinTone, inSaunaMediumLightSkinTone, inSaunaMediumSkinTone, inSaunaMediumDarkSkinTone, inSaunaDarkSkinTone, inSaunaMale, inSaunaLightSkinToneMale, inSaunaMediumLightSkinToneMale, inSaunaMediumSkinToneMale, inSaunaMediumDarkSkinToneMale, inSaunaDarkSkinToneMale, inSaunaFemale, inSaunaLightSkinToneFemale, inSaunaMediumLightSkinToneFemale, inSaunaMediumSkinToneFemale, inSaunaMediumDarkSkinToneFemale, inSaunaDarkSkinToneFemale } from ".";



export const sexes = /*@__PURE__*/[
    female,
    male,
    transgenderSymbol
];

export const skinTones = /*@__PURE__*/[
    lightSkinTone,
    mediumLightSkinTone,
    mediumSkinTone,
    mediumDarkSkinTone,
    darkSkinTone
];

export const hairStyles = /*@__PURE__*/[
    redHair,
    curlyHair,
    whiteHair,
    bald
];

export const allFrowning = /*@__PURE__*/[
    frowning,
    frowningLightSkinTone,
    frowningMediumLightSkinTone,
    frowningMediumSkinTone,
    frowningMediumDarkSkinTone,
    frowningDarkSkinTone
];

export const allFrowningGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64D}\uDE4D", "Frowning", ...allFrowning);

export const allFrowningMale = /*@__PURE__*/ [
    frowningMale,
    frowningLightSkinToneMale,
    frowningMediumLightSkinToneMale,
    frowningMediumSkinToneMale,
    frowningMediumDarkSkinToneMale,
    frowningDarkSkinToneMale
];

export const allFrowningMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64D}\uDE4D\u200D\u2642\uFE0F", "Frowning: Male", ...allFrowningMale);

export const allFrowningFemale = /*@__PURE__*/[
    frowningFemale,
    frowningLightSkinToneFemale,
    frowningMediumLightSkinToneFemale,
    frowningMediumSkinToneFemale,
    frowningMediumDarkSkinToneFemale,
    frowningDarkSkinToneFemale
];

export const allFrowningFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64D}\uDE4D\u200D\u2640\uFE0F", "Frowning: Female", ...allFrowningFemale);

export const allFrowners = /*@__PURE__*/[
    allFrowningGroup,
    allFrowningMaleGroup,
    allFrowningFemaleGroup
];

export const allFrownersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64D}\uDE4D", "Frowning", ...allFrowners);

export const allPouting = /*@__PURE__*/[
    pouting,
    poutingLightSkinTone,
    poutingMediumLightSkinTone,
    poutingMediumSkinTone,
    poutingMediumDarkSkinTone,
    poutingDarkSkinTone
];

export const allPoutingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64E}\uDE4E", "Pouting", ...allPouting);

export const allPoutingMale = /*@__PURE__*/[
    poutingMale,
    poutingLightSkinToneMale,
    poutingMediumLightSkinToneMale,
    poutingMediumSkinToneMale,
    poutingMediumDarkSkinToneMale,
    poutingDarkSkinToneMale
];

export const allPoutingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64E}\uDE4E\u200D\u2642\uFE0F", "Pouting: Male", ...allPoutingMale);

export const allPoutingFemale = /*@__PURE__*/[
    poutingFemale,
    poutingLightSkinToneFemale,
    poutingMediumLightSkinToneFemale,
    poutingMediumSkinToneFemale,
    poutingMediumDarkSkinToneFemale,
    poutingDarkSkinToneFemale
];

export const allPoutingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64E}\uDE4E\u200D\u2640\uFE0F", "Pouting: Female", ...allPoutingFemale);

export const allPouters = /*@__PURE__*/ [
    allPoutingGroup,
    allPoutingMaleGroup,
    allPoutingFemaleGroup
];

export const allPoutersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64E}\uDE4E", "Pouting", ...allPouters);

export const allGesturingNO = /*@__PURE__*/[
    gesturingNO,
    gesturingNOLightSkinTone,
    gesturingNOMediumLightSkinTone,
    gesturingNOMediumSkinTone,
    gesturingNOMediumDarkSkinTone,
    gesturingNODarkSkinTone
];

export const allGesturingNOGroup = /*@__PURE__*/ new EmojiGroup("\u{1F645}\uDE45", "Gesturing NO", ...allGesturingNO);

export const allGesturingNOMale = /*@__PURE__*/[
    gesturingNOMale,
    gesturingNOLightSkinToneMale,
    gesturingNOMediumLightSkinToneMale,
    gesturingNOMediumSkinToneMale,
    gesturingNOMediumDarkSkinToneMale,
    gesturingNODarkSkinToneMale
];

export const allGesturingNOMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F645}\uDE45\u200D\u2642\uFE0F", "Gesturing NO: Male", ...allGesturingNOMale);

export const allGesturingNOFemale = /*@__PURE__*/[
    gesturingNOFemale,
    gesturingNOLightSkinToneFemale,
    gesturingNOMediumLightSkinToneFemale,
    gesturingNOMediumSkinToneFemale,
    gesturingNOMediumDarkSkinToneFemale,
    gesturingNODarkSkinToneFemale
];

export const allGesturingNOFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F645}\uDE45\u200D\u2640\uFE0F", "Gesturing NO: Female", ...allGesturingNOFemale);

export const allNoGuesturerersGroup = /*@__PURE__*/[
    allGesturingNOGroup,
    allGesturingNOMaleGroup,
    allGesturingNOFemaleGroup
];

export const allNoGuesturerersGroupGroup = /*@__PURE__*/ new EmojiGroup("\u{1F645}\uDE45", "Gesturing NO", ...allNoGuesturerersGroup);

export const allGesturingOK = /*@__PURE__*/[
    gesturingOK,
    gesturingOKLightSkinTone,
    gesturingOKMediumLightSkinTone,
    gesturingOKMediumSkinTone,
    gesturingOKMediumDarkSkinTone,
    gesturingOKDarkSkinTone
];

export const allGesturingOKGroup = /*@__PURE__*/ new EmojiGroup("\u{1F646}\uDE46", "Gesturing OK", ...allGesturingOK);

export const allGesturingOKMale = /*@__PURE__*/[
    gesturingOKMale,
    gesturingOKLightSkinToneMale,
    gesturingOKMediumLightSkinToneMale,
    gesturingOKMediumSkinToneMale,
    gesturingOKMediumDarkSkinToneMale,
    gesturingOKDarkSkinToneMale
];

export const allGesturingOKMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F646}\uDE46\u200D\u2642\uFE0F", "Gesturing OK: Male", ...allGesturingOKMale);

export const allGesturingOKFemale = /*@__PURE__*/[
    gesturingOKFemale,
    gesturingOKLightSkinToneFemale,
    gesturingOKMediumLightSkinToneFemale,
    gesturingOKMediumSkinToneFemale,
    gesturingOKMediumDarkSkinToneFemale,
    gesturingOKDarkSkinToneFemale
];

export const allGesturingOKFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F646}\uDE46\u200D\u2640\uFE0F", "Gesturing OK: Female", ...allGesturingOKFemale);

export const allOKGesturerersGroup = /*@__PURE__*/[
    allGesturingOKGroup,
    allGesturingOKMaleGroup,
    allGesturingOKFemaleGroup
];

export const allOKGesturerersGroupGroup = /*@__PURE__*/ new EmojiGroup("\u{1F646}\uDE46", "Gesturing OK", ...allOKGesturerersGroup);

export const allTippingHand = /*@__PURE__*/[
    tippingHand,
    tippingHandLightSkinTone,
    tippingHandMediumLightSkinTone,
    tippingHandMediumSkinTone,
    tippingHandMediumDarkSkinTone,
    tippingHandDarkSkinTone
];

export const allTippingHandGroup = /*@__PURE__*/ new EmojiGroup("\u{1F481}\uDC81", "Tipping Hand", ...allTippingHand);

export const allTippingHandMale = /*@__PURE__*/[
    tippingHandMale,
    tippingHandLightSkinToneMale,
    tippingHandMediumLightSkinToneMale,
    tippingHandMediumSkinToneMale,
    tippingHandMediumDarkSkinToneMale,
    tippingHandDarkSkinToneMale
];

export const allTippingHandMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F481}\uDC81\u200D\u2642\uFE0F", "Tipping Hand: Male", ...allTippingHandMale);

export const allTippingHandFemale = /*@__PURE__*/[
    tippingHandFemale,
    tippingHandLightSkinToneFemale,
    tippingHandMediumLightSkinToneFemale,
    tippingHandMediumSkinToneFemale,
    tippingHandMediumDarkSkinToneFemale,
    tippingHandDarkSkinToneFemale
];

export const allTippingHandFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F481}\uDC81\u200D\u2640\uFE0F", "Tipping Hand: Female", ...allTippingHandFemale);

export const allHandTippersGroup = /*@__PURE__*/[
    allTippingHandGroup,
    allTippingHandMaleGroup,
    allTippingHandFemaleGroup
];

export const allHandTippersGroupGroup = /*@__PURE__*/ new EmojiGroup("\u{1F481}\uDC81", "Tipping Hand", ...allHandTippersGroup);

export const allRaisingHand = /*@__PURE__*/[
    raisingHand,
    raisingHandLightSkinTone,
    raisingHandMediumLightSkinTone,
    raisingHandMediumSkinTone,
    raisingHandMediumDarkSkinTone,
    raisingHandDarkSkinTone
];

export const allRaisingHandGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64B}\uDE4B", "Raising Hand", ...allRaisingHand);

export const allRaisingHandMale = /*@__PURE__*/[
    raisingHandMale,
    raisingHandLightSkinToneMale,
    raisingHandMediumLightSkinToneMale,
    raisingHandMediumSkinToneMale,
    raisingHandMediumDarkSkinToneMale,
    raisingHandDarkSkinToneMale
];

export const allRaisingHandMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64B}\uDE4B\u200D\u2642\uFE0F", "Raising Hand: Male", ...allRaisingHandMale);

export const allRaisingHandFemale = /*@__PURE__*/[
    raisingHandFemale,
    raisingHandLightSkinToneFemale,
    raisingHandMediumLightSkinToneFemale,
    raisingHandMediumSkinToneFemale,
    raisingHandMediumDarkSkinToneFemale,
    raisingHandDarkSkinToneFemale
];

export const allRaisingHandFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64B}\uDE4B\u200D\u2640\uFE0F", "Raising Hand: Female", ...allRaisingHandFemale);

export const allHandRaisers = /*@__PURE__*/ [
    allRaisingHandGroup,
    allRaisingHandMaleGroup,
    allRaisingHandFemaleGroup
];

export const allHandRaisersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F64B}\uDE4B", "Raising Hand", ...allHandRaisers);

export const allBowing = /*@__PURE__*/ [
    bowing,
    bowingLightSkinTone,
    bowingMediumLightSkinTone,
    bowingMediumSkinTone,
    bowingMediumDarkSkinTone,
    bowingDarkSkinTone
];

export const allBowingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F647}\uDE47", "Bowing", ...allBowing);

export const allBowingMale = /*@__PURE__*/ [
    bowingMale,
    bowingLightSkinToneMale,
    bowingMediumLightSkinToneMale,
    bowingMediumSkinToneMale,
    bowingMediumDarkSkinToneMale,
    bowingDarkSkinToneMale
];

export const allBowingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F647}\uDE47\u200D\u2642\uFE0F", "Bowing: Male", ...allBowingMale);

export const allBowingFemale = /*@__PURE__*/ [
    bowingFemale,
    bowingLightSkinToneFemale,
    bowingMediumLightSkinToneFemale,
    bowingMediumSkinToneFemale,
    bowingMediumDarkSkinToneFemale,
    bowingDarkSkinToneFemale
];

export const allBowingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F647}\uDE47\u200D\u2640\uFE0F", "Bowing: Female", ...allBowingFemale);

export const allBowers = /*@__PURE__*/ [
    allBowingGroup,
    allBowingMaleGroup,
    allBowingFemaleGroup
];

export const allBowersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F647}\uDE47", "Bowing", ...allBowers);

export const allFacepalming = /*@__PURE__*/ [
    facepalming,
    facepalmingLightSkinTone,
    facepalmingMediumLightSkinTone,
    facepalmingMediumSkinTone,
    facepalmingMediumDarkSkinTone,
    facepalmingDarkSkinTone
];

export const allFacepalmingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F926}\uDD26", "Facepalming", ...allFacepalming);

export const allFacepalmingMale = /*@__PURE__*/ [
    facepalmingMale,
    facepalmingLightSkinToneMale,
    facepalmingMediumLightSkinToneMale,
    facepalmingMediumSkinToneMale,
    facepalmingMediumDarkSkinToneMale,
    facepalmingDarkSkinToneMale
];

export const allFacepalmingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F926}\uDD26\u200D\u2642\uFE0F", "Facepalming: Male", ...allFacepalmingMale);

export const allFacepalmingFemale = /*@__PURE__*/ [
    facepalmingFemale,
    facepalmingLightSkinToneFemale,
    facepalmingMediumLightSkinToneFemale,
    facepalmingMediumSkinToneFemale,
    facepalmingMediumDarkSkinToneFemale,
    facepalmingDarkSkinToneFemale
];

export const allFacepalmingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F926}\uDD26\u200D\u2640\uFE0F", "Facepalming: Female", ...allFacepalmingFemale);

export const allFacepalmers = /*@__PURE__*/ [
    allFacepalmingGroup,
    allFacepalmingMaleGroup,
    allFacepalmingFemaleGroup
];

export const allFacepalmersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F926}\uDD26", "Facepalming", ...allFacepalmers);

export const allShrugging = /*@__PURE__*/ [
    shrugging,
    shruggingLightSkinTone,
    shruggingMediumLightSkinTone,
    shruggingMediumSkinTone,
    shruggingMediumDarkSkinTone,
    shruggingDarkSkinTone
];

export const allShruggingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F937}\uDD37", "Shrugging", ...allShrugging);

export const allShruggingMale = /*@__PURE__*/ [
    shruggingMale,
    shruggingLightSkinToneMale,
    shruggingMediumLightSkinToneMale,
    shruggingMediumSkinToneMale,
    shruggingMediumDarkSkinToneMale,
    shruggingDarkSkinToneMale
];

export const allShruggingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F937}\uDD37\u200D\u2642\uFE0F", "Shrugging: Male", ...allShruggingMale);

export const allShruggingFemale = /*@__PURE__*/ [
    shruggingFemale,
    shruggingLightSkinToneFemale,
    shruggingMediumLightSkinToneFemale,
    shruggingMediumSkinToneFemale,
    shruggingMediumDarkSkinToneFemale,
    shruggingDarkSkinToneFemale
];

export const allShruggingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F937}\uDD37\u200D\u2640\uFE0F", "Shrugging: Female", ...allShruggingFemale);

export const allShruggers = /*@__PURE__*/ [
    allShruggingGroup,
    allShruggingMaleGroup,
    allShruggingFemaleGroup
];

export const allShruggersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F937}\uDD37", "Shrugging", ...allShruggers);

export const allCantHear = /*@__PURE__*/ [
    cantHear,
    cantHearLightSkinTone,
    cantHearMediumLightSkinTone,
    cantHearMediumSkinTone,
    cantHearMediumDarkSkinTone,
    cantHearDarkSkinTone
];

export const allCantHearGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CF}\uDDCF", "Can't Hear", ...allCantHear);

export const allCantHearMale = /*@__PURE__*/ [
    cantHearMale,
    cantHearLightSkinToneMale,
    cantHearMediumLightSkinToneMale,
    cantHearMediumSkinToneMale,
    cantHearMediumDarkSkinToneMale,
    cantHearDarkSkinToneMale
];

export const allCantHearMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CF}\uDDCF\u200D\u2642\uFE0F", "Can't Hear: Male", ...allCantHearMale);

export const allCantHearFemale = /*@__PURE__*/ [
    cantHearFemale,
    cantHearLightSkinToneFemale,
    cantHearMediumLightSkinToneFemale,
    cantHearMediumSkinToneFemale,
    cantHearMediumDarkSkinToneFemale,
    cantHearDarkSkinToneFemale
];

export const allCantHearFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CF}\uDDCF\u200D\u2640\uFE0F", "Can't Hear: Female", ...allCantHearFemale);

export const allCantHearers = /*@__PURE__*/ [
    allCantHearGroup,
    allCantHearMaleGroup,
    allCantHearFemaleGroup
];

export const allCantHearersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CF}\uDDCF", "Can't Hear", ...allCantHearers);

export const allGettingMassage = /*@__PURE__*/ [
    gettingMassage,
    gettingMassageLightSkinTone,
    gettingMassageMediumLightSkinTone,
    gettingMassageMediumSkinTone,
    gettingMassageMediumDarkSkinTone,
    gettingMassageDarkSkinTone
];

export const allGettingMassageGroup = /*@__PURE__*/ new EmojiGroup("\u{1F486}\uDC86", "Getting Massage", ...allGettingMassage);

export const allGettingMassageMale = /*@__PURE__*/ [
    gettingMassageMale,
    gettingMassageLightSkinToneMale,
    gettingMassageMediumLightSkinToneMale,
    gettingMassageMediumSkinToneMale,
    gettingMassageMediumDarkSkinToneMale,
    gettingMassageDarkSkinToneMale
];

export const allGettingMassageMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F486}\uDC86\u200D\u2642\uFE0F", "Getting Massage: Male", ...allGettingMassageMale);

export const allGettingMassageFemale = /*@__PURE__*/ [
    gettingMassageFemale,
    gettingMassageLightSkinToneFemale,
    gettingMassageMediumLightSkinToneFemale,
    gettingMassageMediumSkinToneFemale,
    gettingMassageMediumDarkSkinToneFemale,
    gettingMassageDarkSkinToneFemale
];

export const allGettingMassageFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F486}\uDC86\u200D\u2640\uFE0F", "Getting Massage: Female", ...allGettingMassageFemale);

export const allGettingMassaged = /*@__PURE__*/ [
    allGettingMassageGroup,
    allGettingMassageMaleGroup,
    allGettingMassageFemaleGroup
];

export const allGettingMassagedGroup = /*@__PURE__*/ new EmojiGroup("\u{1F486}\uDC86", "Getting Massage", ...allGettingMassaged);

export const allGettingHaircut = /*@__PURE__*/ [
    gettingHaircut,
    gettingHaircutLightSkinTone,
    gettingHaircutMediumLightSkinTone,
    gettingHaircutMediumSkinTone,
    gettingHaircutMediumDarkSkinTone,
    gettingHaircutDarkSkinTone
];

export const allGettingHaircutGroup = /*@__PURE__*/ new EmojiGroup("\u{1F487}\uDC87", "Getting Haircut", ...allGettingHaircut);

export const allGettingHaircutMale = /*@__PURE__*/ [
    gettingHaircutMale,
    gettingHaircutLightSkinToneMale,
    gettingHaircutMediumLightSkinToneMale,
    gettingHaircutMediumSkinToneMale,
    gettingHaircutMediumDarkSkinToneMale,
    gettingHaircutDarkSkinToneMale
];

export const allGettingHaircutMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F487}\uDC87\u200D\u2642\uFE0F", "Getting Haircut: Male", ...allGettingHaircutMale);

export const allGettingHaircutFemale = /*@__PURE__*/ [
    gettingHaircutFemale,
    gettingHaircutLightSkinToneFemale,
    gettingHaircutMediumLightSkinToneFemale,
    gettingHaircutMediumSkinToneFemale,
    gettingHaircutMediumDarkSkinToneFemale,
    gettingHaircutDarkSkinToneFemale
];

export const allGettingHaircutFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F487}\uDC87\u200D\u2640\uFE0F", "Getting Haircut: Female", ...allGettingHaircutFemale);

export const allHairCutters = /*@__PURE__*/ [
    allGettingHaircutGroup,
    allGettingHaircutMaleGroup,
    allGettingHaircutFemaleGroup
];

export const allHairCuttersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F487}\uDC87", "Getting Haircut", ...allHairCutters);

export const allConstructionWorker = /*@__PURE__*/ [
    constructionWorker,
    constructionWorkerLightSkinTone,
    constructionWorkerMediumLightSkinTone,
    constructionWorkerMediumSkinTone,
    constructionWorkerMediumDarkSkinTone,
    constructionWorkerDarkSkinTone
];

export const allConstructionWorkerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F477}\uDC77", "Construction Worker", ...allConstructionWorker);

export const allConstructionWorkerMale = /*@__PURE__*/ [
    constructionWorkerMale,
    constructionWorkerLightSkinToneMale,
    constructionWorkerMediumLightSkinToneMale,
    constructionWorkerMediumSkinToneMale,
    constructionWorkerMediumDarkSkinToneMale,
    constructionWorkerDarkSkinToneMale
];

export const allConstructionWorkerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F477}\uDC77\u200D\u2642\uFE0F", "Construction Worker: Male", ...allConstructionWorkerMale);

export const allConstructionWorkerFemale = /*@__PURE__*/ [
    constructionWorkerFemale,
    constructionWorkerLightSkinToneFemale,
    constructionWorkerMediumLightSkinToneFemale,
    constructionWorkerMediumSkinToneFemale,
    constructionWorkerMediumDarkSkinToneFemale,
    constructionWorkerDarkSkinToneFemale
];

export const allConstructionWorkerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F477}\uDC77\u200D\u2640\uFE0F", "Construction Worker: Female", ...allConstructionWorkerFemale);

export const allConstructionWorkers = /*@__PURE__*/ [
    allConstructionWorkerGroup,
    allConstructionWorkerMaleGroup,
    allConstructionWorkerFemaleGroup
];

export const allConstructionWorkersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F477}\uDC77", "Construction Worker", ...allConstructionWorkers);

export const allGuard = /*@__PURE__*/ [
    guard,
    guardLightSkinTone,
    guardMediumLightSkinTone,
    guardMediumSkinTone,
    guardMediumDarkSkinTone,
    guardDarkSkinTone
];

export const allGuardGroup = /*@__PURE__*/ new EmojiGroup("\u{1F482}\uDC82", "Guard", ...allGuard);

export const allGuardMale = /*@__PURE__*/ [
    guardMale,
    guardLightSkinToneMale,
    guardMediumLightSkinToneMale,
    guardMediumSkinToneMale,
    guardMediumDarkSkinToneMale,
    guardDarkSkinToneMale
];

export const allGuardMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F482}\uDC82\u200D\u2642\uFE0F", "Guard: Male", ...allGuardMale);

export const allGuardFemale = /*@__PURE__*/ [
    guardFemale,
    guardLightSkinToneFemale,
    guardMediumLightSkinToneFemale,
    guardMediumSkinToneFemale,
    guardMediumDarkSkinToneFemale,
    guardDarkSkinToneFemale
];

export const allGuardFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F482}\uDC82\u200D\u2640\uFE0F", "Guard: Female", ...allGuardFemale);

export const allGuards = /*@__PURE__*/ [
    allGuardGroup,
    allGuardMaleGroup,
    allGuardFemaleGroup
];

export const allGuardsGroup = /*@__PURE__*/ new EmojiGroup("\u{1F482}\uDC82", "Guard", ...allGuards);

export const allSpy = /*@__PURE__*/ [
    spy,
    spyLightSkinTone,
    spyMediumLightSkinTone,
    spyMediumSkinTone,
    spyMediumDarkSkinTone,
    spyDarkSkinTone
];

export const allSpyGroup = /*@__PURE__*/ new EmojiGroup("\u{1F575}\uDD75", "Spy", ...allSpy);

export const allSpyMale = /*@__PURE__*/ [
    spyMale,
    spyLightSkinToneMale,
    spyMediumLightSkinToneMale,
    spyMediumSkinToneMale,
    spyMediumDarkSkinToneMale,
    spyDarkSkinToneMale
];

export const allSpyMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F575}\uDD75\u200D\u2642\uFE0F", "Spy: Male", ...allSpyMale);

export const allSpyFemale = /*@__PURE__*/ [
    spyFemale,
    spyLightSkinToneFemale,
    spyMediumLightSkinToneFemale,
    spyMediumSkinToneFemale,
    spyMediumDarkSkinToneFemale,
    spyDarkSkinToneFemale
];

export const allSpyFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F575}\uDD75\u200D\u2640\uFE0F", "Spy: Female", ...allSpyFemale);

export const allSpies = /*@__PURE__*/ [
    allSpyGroup,
    allSpyMaleGroup,
    allSpyFemaleGroup
];

export const allSpiesGroup = /*@__PURE__*/ new EmojiGroup("\u{1F575}\uDD75", "Spy", ...allSpies);

export const allPolice = /*@__PURE__*/ [
    police,
    policeLightSkinTone,
    policeMediumLightSkinTone,
    policeMediumSkinTone,
    policeMediumDarkSkinTone,
    policeDarkSkinTone
];

export const allPoliceGroup = /*@__PURE__*/ new EmojiGroup("\u{1F46E}\uDC6E", "Police", ...allPolice);

export const allPoliceMale = /*@__PURE__*/ [
    policeMale,
    policeLightSkinToneMale,
    policeMediumLightSkinToneMale,
    policeMediumSkinToneMale,
    policeMediumDarkSkinToneMale,
    policeDarkSkinToneMale
];

export const allPoliceMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F46E}\uDC6E\u200D\u2642\uFE0F", "Police: Male", ...allPoliceMale);

export const allPoliceFemale = /*@__PURE__*/ [
    policeFemale,
    policeLightSkinToneFemale,
    policeMediumLightSkinToneFemale,
    policeMediumSkinToneFemale,
    policeMediumDarkSkinToneFemale,
    policeDarkSkinToneFemale
];

export const allPoliceFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F46E}\uDC6E\u200D\u2640\uFE0F", "Police: Female", ...allPoliceFemale);

export const allCops = /*@__PURE__*/ [
    allPoliceGroup,
    allPoliceMaleGroup,
    allPoliceFemaleGroup
];

export const allCopsGroup = /*@__PURE__*/ new EmojiGroup("\u{1F46E}\uDC6E", "Police", ...allCops);

export const allWearingTurban = /*@__PURE__*/ [
    wearingTurban,
    wearingTurbanLightSkinTone,
    wearingTurbanMediumLightSkinTone,
    wearingTurbanMediumSkinTone,
    wearingTurbanMediumDarkSkinTone,
    wearingTurbanDarkSkinTone
];

export const allWearingTurbanGroup = /*@__PURE__*/ new EmojiGroup("\u{1F473}\uDC73", "Wearing Turban", ...allWearingTurban);

export const allWearingTurbanMale = /*@__PURE__*/ [
    wearingTurbanMale,
    wearingTurbanLightSkinToneMale,
    wearingTurbanMediumLightSkinToneMale,
    wearingTurbanMediumSkinToneMale,
    wearingTurbanMediumDarkSkinToneMale,
    wearingTurbanDarkSkinToneMale
];

export const allWearingTurbanMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F473}\uDC73\u200D\u2642\uFE0F", "Wearing Turban: Male", ...allWearingTurbanMale);

export const allWearingTurbanFemale = /*@__PURE__*/ [
    wearingTurbanFemale,
    wearingTurbanLightSkinToneFemale,
    wearingTurbanMediumLightSkinToneFemale,
    wearingTurbanMediumSkinToneFemale,
    wearingTurbanMediumDarkSkinToneFemale,
    wearingTurbanDarkSkinToneFemale
];

export const allWearingTurbanFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F473}\uDC73\u200D\u2640\uFE0F", "Wearing Turban: Female", ...allWearingTurbanFemale);

export const allTurbanWearers = /*@__PURE__*/ [
    allWearingTurbanGroup,
    allWearingTurbanMaleGroup,
    allWearingTurbanFemaleGroup
];

export const allTurbanWearersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F473}\uDC73", "Wearing Turban", ...allTurbanWearers);

export const allSuperhero = /*@__PURE__*/ [
    superhero,
    superheroLightSkinTone,
    superheroMediumLightSkinTone,
    superheroMediumSkinTone,
    superheroMediumDarkSkinTone,
    superheroDarkSkinTone
];

export const allSuperheroGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B8}\uDDB8", "Superhero", ...allSuperhero);

export const allSuperheroMale = /*@__PURE__*/ [
    superheroMale,
    superheroLightSkinToneMale,
    superheroMediumLightSkinToneMale,
    superheroMediumSkinToneMale,
    superheroMediumDarkSkinToneMale,
    superheroDarkSkinToneMale
];

export const allSuperheroMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B8}\uDDB8\u200D\u2642\uFE0F", "Superhero: Male", ...allSuperheroMale);

export const allSuperheroFemale = /*@__PURE__*/ [
    superheroFemale,
    superheroLightSkinToneFemale,
    superheroMediumLightSkinToneFemale,
    superheroMediumSkinToneFemale,
    superheroMediumDarkSkinToneFemale,
    superheroDarkSkinToneFemale
];

export const allSuperheroFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B8}\uDDB8\u200D\u2640\uFE0F", "Superhero: Female", ...allSuperheroFemale);

export const allSuperheroes = /*@__PURE__*/ [
    allSuperheroGroup,
    allSuperheroMaleGroup,
    allSuperheroFemaleGroup
];

export const allSuperheroesGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B8}\uDDB8", "Superhero", ...allSuperheroes);

export const allSupervillain = /*@__PURE__*/ [
    supervillain,
    supervillainLightSkinTone,
    supervillainMediumLightSkinTone,
    supervillainMediumSkinTone,
    supervillainMediumDarkSkinTone,
    supervillainDarkSkinTone
];

export const allSupervillainGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B9}\uDDB9", "Supervillain", ...allSupervillain);

export const allSupervillainMale = /*@__PURE__*/ [
    supervillainMale,
    supervillainLightSkinToneMale,
    supervillainMediumLightSkinToneMale,
    supervillainMediumSkinToneMale,
    supervillainMediumDarkSkinToneMale,
    supervillainDarkSkinToneMale
];

export const allSupervillainMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B9}\uDDB9\u200D\u2642\uFE0F", "Supervillain: Male", ...allSupervillainMale);

export const allSupervillainFemale = /*@__PURE__*/ [
    supervillainFemale,
    supervillainLightSkinToneFemale,
    supervillainMediumLightSkinToneFemale,
    supervillainMediumSkinToneFemale,
    supervillainMediumDarkSkinToneFemale,
    supervillainDarkSkinToneFemale
];

export const allSupervillainFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B9}\uDDB9\u200D\u2640\uFE0F", "Supervillain: Female", ...allSupervillainFemale);

export const allSupervillains = /*@__PURE__*/ [
    allSupervillainGroup,
    allSupervillainMaleGroup,
    allSupervillainFemaleGroup
];

export const allSupervillainsGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9B9}\uDDB9", "Supervillain", ...allSupervillains);

export const allMage = /*@__PURE__*/ [
    mage,
    mageLightSkinTone,
    mageMediumLightSkinTone,
    mageMediumSkinTone,
    mageMediumDarkSkinTone,
    mageDarkSkinTone
];

export const allMageGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D9}\uDDD9", "Mage", ...allMage);

export const allMageMale = /*@__PURE__*/ [
    mageMale,
    mageLightSkinToneMale,
    mageMediumLightSkinToneMale,
    mageMediumSkinToneMale,
    mageMediumDarkSkinToneMale,
    mageDarkSkinToneMale
];

export const allMageMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D9}\uDDD9\u200D\u2642\uFE0F", "Mage: Male", ...allMageMale);

export const allMageFemale = /*@__PURE__*/ [
    mageFemale,
    mageLightSkinToneFemale,
    mageMediumLightSkinToneFemale,
    mageMediumSkinToneFemale,
    mageMediumDarkSkinToneFemale,
    mageDarkSkinToneFemale
];

export const allMageFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D9}\uDDD9\u200D\u2640\uFE0F", "Mage: Female", ...allMageFemale);

export const allMages = /*@__PURE__*/ [
    allMageGroup,
    allMageMaleGroup,
    allMageFemaleGroup
];

export const allMagesGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D9}\uDDD9", "Mage", ...allMages);

export const allFairy = /*@__PURE__*/ [
    fairy,
    fairyLightSkinTone,
    fairyMediumLightSkinTone,
    fairyMediumSkinTone,
    fairyMediumDarkSkinTone,
    fairyDarkSkinTone
];

export const allFairyGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DA}\uDDDA", "Fairy", ...allFairy);

export const allFairyMale = /*@__PURE__*/ [
    fairyMale,
    fairyLightSkinToneMale,
    fairyMediumLightSkinToneMale,
    fairyMediumSkinToneMale,
    fairyMediumDarkSkinToneMale,
    fairyDarkSkinToneMale
];

export const allFairyMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DA}\uDDDA\u200D\u2642\uFE0F", "Fairy: Male", ...allFairyMale);

export const allFairyFemale = /*@__PURE__*/ [
    fairyFemale,
    fairyLightSkinToneFemale,
    fairyMediumLightSkinToneFemale,
    fairyMediumSkinToneFemale,
    fairyMediumDarkSkinToneFemale,
    fairyDarkSkinToneFemale
];

export const allFairyFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DA}\uDDDA\u200D\u2640\uFE0F", "Fairy: Female", ...allFairyFemale);

export const allFairies = /*@__PURE__*/ [
    allFairyGroup,
    allFairyMaleGroup,
    allFairyFemaleGroup
];

export const allFairiesGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DA}\uDDDA", "Fairy", ...allFairies);

export const allVampire = /*@__PURE__*/ [
    vampire,
    vampireLightSkinTone,
    vampireMediumLightSkinTone,
    vampireMediumSkinTone,
    vampireMediumDarkSkinTone,
    vampireDarkSkinTone
];

export const allVampireGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DB}\uDDDB", "Vampire", ...allVampire);

export const allVampireMale = /*@__PURE__*/ [
    vampireMale,
    vampireLightSkinToneMale,
    vampireMediumLightSkinToneMale,
    vampireMediumSkinToneMale,
    vampireMediumDarkSkinToneMale,
    vampireDarkSkinToneMale
];

export const allVampireMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DB}\uDDDB\u200D\u2642\uFE0F", "Vampire: Male", ...allVampireMale);

export const allVampireFemale = /*@__PURE__*/ [
    vampireFemale,
    vampireLightSkinToneFemale,
    vampireMediumLightSkinToneFemale,
    vampireMediumSkinToneFemale,
    vampireMediumDarkSkinToneFemale,
    vampireDarkSkinToneFemale
];

export const allVampireFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DB}\uDDDB\u200D\u2640\uFE0F", "Vampire: Female", ...allVampireFemale);

export const allVampires = /*@__PURE__*/ [
    allVampireGroup,
    allVampireMaleGroup,
    allVampireFemaleGroup
];

export const allVampiresGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DB}\uDDDB", "Vampire", ...allVampires);

export const allMerperson = /*@__PURE__*/ [
    merperson,
    merpersonLightSkinTone,
    merpersonMediumLightSkinTone,
    merpersonMediumSkinTone,
    merpersonMediumDarkSkinTone,
    merpersonDarkSkinTone
];

export const allMerpersonGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DC}\uDDDC", "Merperson", ...allMerperson);

export const allMerpersonMale = /*@__PURE__*/ [
    merpersonMale,
    merpersonLightSkinToneMale,
    merpersonMediumLightSkinToneMale,
    merpersonMediumSkinToneMale,
    merpersonMediumDarkSkinToneMale,
    merpersonDarkSkinToneMale
];

export const allMerpersonMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DC}\uDDDC\u200D\u2642\uFE0F", "Merperson: Male", ...allMerpersonMale);

export const allMerpersonFemale = /*@__PURE__*/ [
    merpersonFemale,
    merpersonLightSkinToneFemale,
    merpersonMediumLightSkinToneFemale,
    merpersonMediumSkinToneFemale,
    merpersonMediumDarkSkinToneFemale,
    merpersonDarkSkinToneFemale
];

export const allMerpersonFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DC}\uDDDC\u200D\u2640\uFE0F", "Merperson: Female", ...allMerpersonFemale);

export const allMerpeople = /*@__PURE__*/ [
    allMerpersonGroup,
    allMerpersonMaleGroup,
    allMerpersonFemaleGroup
];

export const allMerpeopleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DC}\uDDDC", "Merperson", ...allMerpeople);

export const allElf = /*@__PURE__*/ [
    elf,
    elfLightSkinTone,
    elfMediumLightSkinTone,
    elfMediumSkinTone,
    elfMediumDarkSkinTone,
    elfDarkSkinTone
];

export const allElfGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DD}\uDDDD", "Elf", ...allElf);

export const allElfMale = /*@__PURE__*/ [
    elfMale,
    elfLightSkinToneMale,
    elfMediumLightSkinToneMale,
    elfMediumSkinToneMale,
    elfMediumDarkSkinToneMale,
    elfDarkSkinToneMale
];

export const allElfMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DD}\uDDDD\u200D\u2642\uFE0F", "Elf: Male", ...allElfMale);

export const allElfFemale = /*@__PURE__*/ [
    elfFemale,
    elfLightSkinToneFemale,
    elfMediumLightSkinToneFemale,
    elfMediumSkinToneFemale,
    elfMediumDarkSkinToneFemale,
    elfDarkSkinToneFemale
];

export const allElfFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DD}\uDDDD\u200D\u2640\uFE0F", "Elf: Female", ...allElfFemale);

export const allElves = /*@__PURE__*/ [
    allElfGroup,
    allElfMaleGroup,
    allElfFemaleGroup
];

export const allElvesGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DD}\uDDDD", "Elf", ...allElves);

export const allWalking = /*@__PURE__*/ [
    walking,
    walkingLightSkinTone,
    walkingMediumLightSkinTone,
    walkingMediumSkinTone,
    walkingMediumDarkSkinTone,
    walkingDarkSkinTone
];

export const allWalkingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B6}\uDEB6", "Walking", ...allWalking);

export const allWalkingMale = /*@__PURE__*/ [
    walkingMale,
    walkingLightSkinToneMale,
    walkingMediumLightSkinToneMale,
    walkingMediumSkinToneMale,
    walkingMediumDarkSkinToneMale,
    walkingDarkSkinToneMale
];

export const allWalkingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B6}\uDEB6\u200D\u2642\uFE0F", "Walking: Male", ...allWalkingMale);

export const allWalkingFemale = /*@__PURE__*/ [
    walkingFemale,
    walkingLightSkinToneFemale,
    walkingMediumLightSkinToneFemale,
    walkingMediumSkinToneFemale,
    walkingMediumDarkSkinToneFemale,
    walkingDarkSkinToneFemale
];

export const allWalkingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B6}\uDEB6\u200D\u2640\uFE0F", "Walking: Female", ...allWalkingFemale);

export const allWalkers = /*@__PURE__*/ [
    allWalkingGroup,
    allWalkingMaleGroup,
    allWalkingFemaleGroup
];

export const allWalkersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B6}\uDEB6", "Walking", ...allWalkers);

export const allStanding = /*@__PURE__*/ [
    standing,
    standingLightSkinTone,
    standingMediumLightSkinTone,
    standingMediumSkinTone,
    standingMediumDarkSkinTone,
    standingDarkSkinTone
];

export const allStandingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CD}\uDDCD", "Standing", ...allStanding);

export const allStandingMale = /*@__PURE__*/ [
    standingMale,
    standingLightSkinToneMale,
    standingMediumLightSkinToneMale,
    standingMediumSkinToneMale,
    standingMediumDarkSkinToneMale,
    standingDarkSkinToneMale
];

export const allStandingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CD}\uDDCD\u200D\u2642\uFE0F", "Standing: Male", ...allStandingMale);

export const allStandingFemale = /*@__PURE__*/ [
    standingFemale,
    standingLightSkinToneFemale,
    standingMediumLightSkinToneFemale,
    standingMediumSkinToneFemale,
    standingMediumDarkSkinToneFemale,
    standingDarkSkinToneFemale
];

export const allStandingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CD}\uDDCD\u200D\u2640\uFE0F", "Standing: Female", ...allStandingFemale);

export const allStanders = /*@__PURE__*/ [
    allStandingGroup,
    allStandingMaleGroup,
    allStandingFemaleGroup
];

export const allStandersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CD}\uDDCD", "Standing", ...allStanders);

export const allKneeling = /*@__PURE__*/ [
    kneeling,
    kneelingLightSkinTone,
    kneelingMediumLightSkinTone,
    kneelingMediumSkinTone,
    kneelingMediumDarkSkinTone,
    kneelingDarkSkinTone
];

export const allKneelingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CE}\uDDCE", "Kneeling", ...allKneeling);

export const allKneelingMale = /*@__PURE__*/ [
    kneelingMale,
    kneelingLightSkinToneMale,
    kneelingMediumLightSkinToneMale,
    kneelingMediumSkinToneMale,
    kneelingMediumDarkSkinToneMale,
    kneelingDarkSkinToneMale
];

export const allKneelingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CE}\uDDCE\u200D\u2642\uFE0F", "Kneeling: Male", ...allKneelingMale);

export const allKneelingFemale = /*@__PURE__*/ [
    kneelingFemale,
    kneelingLightSkinToneFemale,
    kneelingMediumLightSkinToneFemale,
    kneelingMediumSkinToneFemale,
    kneelingMediumDarkSkinToneFemale,
    kneelingDarkSkinToneFemale
];

export const allKneelingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CE}\uDDCE\u200D\u2640\uFE0F", "Kneeling: Female", ...allKneelingFemale);

export const allKneelers = /*@__PURE__*/ [
    allKneelingGroup,
    allKneelingMaleGroup,
    allKneelingFemaleGroup
];

export const allKneelersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9CE}\uDDCE", "Kneeling", ...allKneelers);

export const allRunning = /*@__PURE__*/ [
    running,
    runningLightSkinTone,
    runningMediumLightSkinTone,
    runningMediumSkinTone,
    runningMediumDarkSkinTone,
    runningDarkSkinTone
];

export const allRunningGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C3}\uDFC3", "Running", ...allRunning);

export const allRunningMale = /*@__PURE__*/ [
    runningMale,
    runningLightSkinToneMale,
    runningMediumLightSkinToneMale,
    runningMediumSkinToneMale,
    runningMediumDarkSkinToneMale,
    runningDarkSkinToneMale
];

export const allRunningMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C3}\uDFC3\u200D\u2642\uFE0F", "Running: Male", ...allRunningMale);

export const allRunningFemale = /*@__PURE__*/ [
    runningFemale,
    runningLightSkinToneFemale,
    runningMediumLightSkinToneFemale,
    runningMediumSkinToneFemale,
    runningMediumDarkSkinToneFemale,
    runningDarkSkinToneFemale
];

export const allRunningFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C3}\uDFC3\u200D\u2640\uFE0F", "Running: Female", ...allRunningFemale);

export const allRunners = /*@__PURE__*/ [
    allRunningGroup,
    allRunningMaleGroup,
    allRunningFemaleGroup
];

export const allRunnersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C3}\uDFC3", "Running", ...allRunners);

export const allGesturing = /*@__PURE__*/ [
    allFrownersGroup,
    allPoutersGroup,
    allNoGuesturerersGroupGroup,
    allOKGesturerersGroupGroup,
    allHandTippersGroupGroup,
    allHandRaisersGroup,
    allBowersGroup,
    allFacepalmersGroup,
    allShruggersGroup,
    allCantHearersGroup,
    allGettingMassagedGroup,
    allHairCuttersGroup
];

export const allGesturingGroup = /*@__PURE__*/ new EmojiGroup("\u0047\u0065\u0073\u0074\u0075\u0072\u0065\u0073", "Gestures", ...allGesturing);

export const allBaby = /*@__PURE__*/ [
    baby,
    babyLightSkinTone,
    babyMediumLightSkinTone,
    babyMediumSkinTone,
    babyMediumDarkSkinTone,
    babyDarkSkinTone
];

export const allBabyGroup = /*@__PURE__*/ new EmojiGroup("\u{1F476}\uDC76", "Baby", ...allBaby);

export const allChild = /*@__PURE__*/ [
    child,
    childLightSkinTone,
    childMediumLightSkinTone,
    childMediumSkinTone,
    childMediumDarkSkinTone,
    childDarkSkinTone
];

export const allChildGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D2}\uDDD2", "Child", ...allChild);

export const allBoy = /*@__PURE__*/ [
    boy,
    boyLightSkinTone,
    boyMediumLightSkinTone,
    boyMediumSkinTone,
    boyMediumDarkSkinTone,
    boyDarkSkinTone
];

export const allBoyGroup = /*@__PURE__*/ new EmojiGroup("\u{1F466}\uDC66", "Boy", ...allBoy);

export const allGirl = /*@__PURE__*/ [
    girl,
    girlLightSkinTone,
    girlMediumLightSkinTone,
    girlMediumSkinTone,
    girlMediumDarkSkinTone,
    girlDarkSkinTone
];

export const allGirlGroup = /*@__PURE__*/ new EmojiGroup("\u{1F467}\uDC67", "Girl", ...allGirl);

export const allChildren = /*@__PURE__*/ [
    allChildGroup,
    allBoyGroup,
    allGirlGroup
];

export const allChildrenGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D2}\uDDD2", "Child", ...allChildren);

export const allBlondPerson = /*@__PURE__*/ [
    blondPerson,
    blondPersonLightSkinTone,
    blondPersonMediumLightSkinTone,
    blondPersonMediumSkinTone,
    blondPersonMediumDarkSkinTone,
    blondPersonDarkSkinTone
];

export const allBlondPersonGroup = /*@__PURE__*/ new EmojiGroup("\u{1F471}\uDC71", "Blond Person", ...allBlondPerson);

export const allBlondPersonMale = /*@__PURE__*/ [
    blondPersonMale,
    blondPersonLightSkinToneMale,
    blondPersonMediumLightSkinToneMale,
    blondPersonMediumSkinToneMale,
    blondPersonMediumDarkSkinToneMale,
    blondPersonDarkSkinToneMale
];

export const allBlondPersonMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F471}\uDC71\u200D\u2642\uFE0F", "Blond Person: Male", ...allBlondPersonMale);

export const allBlondPersonFemale = /*@__PURE__*/ [
    blondPersonFemale,
    blondPersonLightSkinToneFemale,
    blondPersonMediumLightSkinToneFemale,
    blondPersonMediumSkinToneFemale,
    blondPersonMediumDarkSkinToneFemale,
    blondPersonDarkSkinToneFemale
];

export const allBlondPersonFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F471}\uDC71\u200D\u2640\uFE0F", "Blond Person: Female", ...allBlondPersonFemale);

export const allBlondePeople = /*@__PURE__*/ [
    allBlondPersonGroup,
    allBlondPersonMaleGroup,
    allBlondPersonFemaleGroup
];

export const allBlondePeopleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F471}\uDC71", "Blond Person", ...allBlondePeople);

export const allPerson = /*@__PURE__*/ [
    person,
    personLightSkinTone,
    personMediumLightSkinTone,
    personMediumSkinTone,
    personMediumDarkSkinTone,
    personDarkSkinTone,
    allBlondPersonGroup,
    allWearingTurbanGroup
];

export const allPersonGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D1}\uDDD1", "Person", ...allPerson);

export const allBeardedMan = /*@__PURE__*/ [
    beardedMan,
    beardedManLightSkinTone,
    beardedManMediumLightSkinTone,
    beardedManMediumSkinTone,
    beardedManMediumDarkSkinTone,
    beardedManDarkSkinTone
];

export const allBeardedManGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D4}\uDDD4", "Bearded Man", ...allBeardedMan);

export const allManWithChineseCap = /*@__PURE__*/ [
    manWithChineseCap,
    manWithChineseCapLightSkinTone,
    manWithChineseCapMediumLightSkinTone,
    manWithChineseCapMediumSkinTone,
    manWithChineseCapMediumDarkSkinTone,
    manWithChineseCapDarkSkinTone
];

export const allManWithChineseCapGroup = /*@__PURE__*/ new EmojiGroup("\u{1F472}\uDC72", "Man With Chinese Cap", ...allManWithChineseCap);

export const allManInTuxedo = /*@__PURE__*/ [
    manInTuxedo,
    manInTuxedoLightSkinTone,
    manInTuxedoMediumLightSkinTone,
    manInTuxedoMediumSkinTone,
    manInTuxedoMediumDarkSkinTone,
    manInTuxedoDarkSkinTone
];

export const allManInTuxedoGroup = /*@__PURE__*/ new EmojiGroup("\u{1F935}\uDD35", "Man in Tuxedo", ...allManInTuxedo);

export const allMan = /*@__PURE__*/ [
    man,
    manLightSkinTone,
    manMediumLightSkinTone,
    manMediumSkinTone,
    manMediumDarkSkinTone,
    manDarkSkinTone
];

export const allManGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68", "Man", ...allMan);

export const allManRedHair = /*@__PURE__*/ [
    manRedHair,
    manLightSkinToneRedHair,
    manMediumLightSkinToneRedHair,
    manMediumSkinToneRedHair,
    manMediumDarkSkinToneRedHair,
    manDarkSkinToneRedHair
];

export const allManRedHairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9B0}\uDDB0", "Man: Red Hair", ...allManRedHair);

export const allManCurlyHair = /*@__PURE__*/ [
    manCurlyHair,
    manLightSkinToneCurlyHair,
    manMediumLightSkinToneCurlyHair,
    manMediumSkinToneCurlyHair,
    manMediumDarkSkinToneCurlyHair,
    manDarkSkinToneCurlyHair
];

export const allManCurlyHairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9B1}\uDDB1", "Man: Curly Hair", ...allManCurlyHair);

export const allManWhiteHair = /*@__PURE__*/ [
    manWhiteHair,
    manLightSkinToneWhiteHair,
    manMediumLightSkinToneWhiteHair,
    manMediumSkinToneWhiteHair,
    manMediumDarkSkinToneWhiteHair,
    manDarkSkinToneWhiteHair
];

export const allManWhiteHairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9B3}\uDDB3", "Man: White Hair", ...allManWhiteHair);

export const allManBald = /*@__PURE__*/ [
    manBald,
    manLightSkinToneBald,
    manMediumLightSkinToneBald,
    manMediumSkinToneBald,
    manMediumDarkSkinToneBald,
    manDarkSkinToneBald
];

export const allManBaldGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9B2}\uDDB2", "Man: Bald", ...allManBald);

export const allMen = /*@__PURE__*/ [
    allManGroup,
    allManRedHairGroup,
    allManCurlyHairGroup,
    allManWhiteHairGroup,
    allManBaldGroup,
    allBlondPersonMaleGroup,
    allBeardedManGroup,
    manInSuitLevitating,
    allManWithChineseCapGroup,
    allWearingTurbanMaleGroup,
    allManInTuxedoGroup
];

export const allMenGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68", "Man", ...allMen);

export const allPregnantWoman = /*@__PURE__*/ [
    pregnantWoman,
    pregnantWomanLightSkinTone,
    pregnantWomanMediumLightSkinTone,
    pregnantWomanMediumSkinTone,
    pregnantWomanMediumDarkSkinTone,
    pregnantWomanDarkSkinTone
];

export const allPregnantWomanGroup = /*@__PURE__*/ new EmojiGroup("\u{1F930}\uDD30", "Pregnant Woman", ...allPregnantWoman);

export const allBreastFeeding = /*@__PURE__*/ [
    breastFeeding,
    breastFeedingLightSkinTone,
    breastFeedingMediumLightSkinTone,
    breastFeedingMediumSkinTone,
    breastFeedingMediumDarkSkinTone,
    breastFeedingDarkSkinTone
];

export const allBreastFeedingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F931}\uDD31", "Breast-Feeding", ...allBreastFeeding);

export const allWomanWithHeadscarf = /*@__PURE__*/ [
    womanWithHeadscarf,
    womanWithHeadscarfLightSkinTone,
    womanWithHeadscarfMediumLightSkinTone,
    womanWithHeadscarfMediumSkinTone,
    womanWithHeadscarfMediumDarkSkinTone,
    womanWithHeadscarfDarkSkinTone
];

export const allWomanWithHeadscarfGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D5}\uDDD5", "Woman With Headscarf", ...allWomanWithHeadscarf);

export const allBrideWithVeil = /*@__PURE__*/ [
    brideWithVeil,
    brideWithVeilLightSkinTone,
    brideWithVeilMediumLightSkinTone,
    brideWithVeilMediumSkinTone,
    brideWithVeilMediumDarkSkinTone,
    brideWithVeilDarkSkinTone
];

export const allBrideWithVeilGroup = /*@__PURE__*/ new EmojiGroup("\u{1F470}\uDC70", "Bride With Veil", ...allBrideWithVeil);

export const allWoman = /*@__PURE__*/ [
    woman,
    womanLightSkinTone,
    womanMediumLightSkinTone,
    womanMediumSkinTone,
    womanMediumDarkSkinTone,
    womanDarkSkinTone
];

export const allWomanGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69", "Woman", ...allWoman);

export const allWomanRedHair = /*@__PURE__*/ [
    womanRedHair,
    womanLightSkinToneRedHair,
    womanMediumLightSkinToneRedHair,
    womanMediumSkinToneRedHair,
    womanMediumDarkSkinToneRedHair,
    womanDarkSkinToneRedHair
];

export const allWomanRedHairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9B0}\uDDB0", "Woman: Red Hair", ...allWomanRedHair);

export const allWomanCurlyHair = /*@__PURE__*/ [
    womanCurlyHair,
    womanLightSkinToneCurlyHair,
    womanMediumLightSkinToneCurlyHair,
    womanMediumSkinToneCurlyHair,
    womanMediumDarkSkinToneCurlyHair,
    womanDarkSkinToneCurlyHair
];

export const allWomanCurlyHairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9B1}\uDDB1", "Woman: Curly Hair", ...allWomanCurlyHair);

export const allWomanWhiteHair = /*@__PURE__*/ [
    womanWhiteHair,
    womanLightSkinToneWhiteHair,
    womanMediumLightSkinToneWhiteHair,
    womanMediumSkinToneWhiteHair,
    womanMediumDarkSkinToneWhiteHair,
    womanDarkSkinToneWhiteHair
];

export const allWomanWhiteHairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9B3}\uDDB3", "Woman: White Hair", ...allWomanWhiteHair);

export const allWomanBald = /*@__PURE__*/ [
    womanBald,
    womanLightSkinToneBald,
    womanMediumLightSkinToneBald,
    womanMediumSkinToneBald,
    womanMediumDarkSkinToneBald,
    womanDarkSkinToneBald
];

export const allWomanBaldGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9B2}\uDDB2", "Woman: Bald", ...allWomanBald);

export const allWomen = /*@__PURE__*/ [
    allWomanGroup,
    allWomanRedHairGroup,
    allWomanCurlyHairGroup,
    allWomanWhiteHairGroup,
    allWomanBaldGroup,
    allBlondPersonFemaleGroup,
    allPregnantWomanGroup,
    allBreastFeedingGroup,
    allWomanWithHeadscarfGroup,
    allWearingTurbanFemaleGroup,
    allBrideWithVeilGroup
];

export const allWomenGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69", "Woman", ...allWomen);

export const allPersons = /*@__PURE__*/ [
    allPersonGroup,
    allMenGroup,
    allWomenGroup
];

export const allPersonsGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D1}\uDDD1", "Adult", ...allPersons);

export const allOlderPerson = /*@__PURE__*/ [
    olderPerson,
    olderPersonLightSkinTone,
    olderPersonMediumLightSkinTone,
    olderPersonMediumSkinTone,
    olderPersonMediumDarkSkinTone,
    olderPersonDarkSkinTone
];

export const allOlderPersonGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D3}\uDDD3", "Older Person", ...allOlderPerson);

export const allOldMan = /*@__PURE__*/ [
    oldMan,
    oldManLightSkinTone,
    oldManMediumLightSkinTone,
    oldManMediumSkinTone,
    oldManMediumDarkSkinTone,
    oldManDarkSkinTone
];

export const allOldManGroup = /*@__PURE__*/ new EmojiGroup("\u{1F474}\uDC74", "Old Man", ...allOldMan);

export const allOldWoman = /*@__PURE__*/ [
    oldWoman,
    oldWomanLightSkinTone,
    oldWomanMediumLightSkinTone,
    oldWomanMediumSkinTone,
    oldWomanMediumDarkSkinTone,
    oldWomanDarkSkinTone
];

export const allOldWomanGroup = /*@__PURE__*/ new EmojiGroup("\u{1F475}\uDC75", "Old Woman", ...allOldWoman);

export const allOlderPersons = /*@__PURE__*/ [
    allOlderPersonGroup,
    allOldManGroup,
    allOldWomanGroup
];

export const allOlderPersonsGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D3}\uDDD3", "Older Person", ...allOlderPersons);

export const allManHealthCare = /*@__PURE__*/ [
    manHealthCare,
    manLightSkinToneHealthCare,
    manMediumLightSkinToneHealthCare,
    manMediumSkinToneHealthCare,
    manMediumDarkSkinToneHealthCare,
    manDarkSkinToneHealthCare
];

export const allManHealthCareGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u2695\uFE0F", "Man: Health Care", ...allManHealthCare);

export const allWomanHealthCare = /*@__PURE__*/ [
    womanHealthCare,
    womanLightSkinToneHealthCare,
    womanMediumLightSkinToneHealthCare,
    womanMediumSkinToneHealthCare,
    womanMediumDarkSkinToneHealthCare,
    womanDarkSkinToneHealthCare
];

export const allWomanHealthCareGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u2695\uFE0F", "Woman: Health Care", ...allWomanHealthCare);

export const allMedical = /*@__PURE__*/ [
    medical,
    allManHealthCareGroup,
    allWomanHealthCareGroup
];

export const allMedicalGroup = /*@__PURE__*/ new EmojiGroup("\u2695\uFE0F", "Medical", ...allMedical);

export const allManStudent = /*@__PURE__*/ [
    manStudent,
    manLightSkinToneStudent,
    manMediumLightSkinToneStudent,
    manMediumSkinToneStudent,
    manMediumDarkSkinToneStudent,
    manDarkSkinToneStudent
];

export const allManStudentGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F393}\uDF93", "Man: Student", ...allManStudent);

export const allWomanStudent = /*@__PURE__*/ [
    womanStudent,
    womanLightSkinToneStudent,
    womanMediumLightSkinToneStudent,
    womanMediumSkinToneStudent,
    womanMediumDarkSkinToneStudent,
    womanDarkSkinToneStudent
];

export const allWomanStudentGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F393}\uDF93", "Woman: Student", ...allWomanStudent);

export const allGraduationCap = /*@__PURE__*/ [
    graduationCap,
    allManStudentGroup,
    allWomanStudentGroup
];

export const allGraduationCapGroup = /*@__PURE__*/ new EmojiGroup("\u{1F393}\uDF93", "Graduation Cap", ...allGraduationCap);

export const allManTeacher = /*@__PURE__*/ [
    manTeacher,
    manLightSkinToneTeacher,
    manMediumLightSkinToneTeacher,
    manMediumSkinToneTeacher,
    manMediumDarkSkinToneTeacher,
    manDarkSkinToneTeacher
];

export const allManTeacherGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F3EB}\uDFEB", "Man: Teacher", ...allManTeacher);

export const allWomanTeacher = /*@__PURE__*/ [
    womanTeacher,
    womanLightSkinToneTeacher,
    womanMediumLightSkinToneTeacher,
    womanMediumSkinToneTeacher,
    womanMediumDarkSkinToneTeacher,
    womanDarkSkinToneTeacher
];

export const allWomanTeacherGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F3EB}\uDFEB", "Woman: Teacher", ...allWomanTeacher);

export const allSchool = /*@__PURE__*/ [
    school,
    allManTeacherGroup,
    allWomanTeacherGroup
];

export const allSchoolGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3EB}\uDFEB", "School", ...allSchool);

export const allManJudge = /*@__PURE__*/ [
    manJudge,
    manLightSkinToneJudge,
    manMediumLightSkinToneJudge,
    manMediumSkinToneJudge,
    manMediumDarkSkinToneJudge,
    manDarkSkinToneJudge
];

export const allManJudgeGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u2696\uFE0F", "Man: Judge", ...allManJudge);

export const allWomanJudge = /*@__PURE__*/ [
    womanJudge,
    womanLightSkinToneJudge,
    womanMediumLightSkinToneJudge,
    womanMediumSkinToneJudge,
    womanMediumDarkSkinToneJudge,
    womanDarkSkinToneJudge
];

export const allWomanJudgeGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u2696\uFE0F", "Woman: Judge", ...allWomanJudge);

export const allBalanceScale = /*@__PURE__*/ [
    balanceScale,
    allManJudgeGroup,
    allWomanJudgeGroup
];

export const allBalanceScaleGroup = /*@__PURE__*/ new EmojiGroup("\u2696\uFE0F", "Balance Scale", ...allBalanceScale);

export const allManFarmer = /*@__PURE__*/ [
    manFarmer,
    manLightSkinToneFarmer,
    manMediumLightSkinToneFarmer,
    manMediumSkinToneFarmer,
    manMediumDarkSkinToneFarmer,
    manDarkSkinToneFarmer
];

export const allManFarmerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F33E}\uDF3E", "Man: Farmer", ...allManFarmer);

export const allWomanFarmer = /*@__PURE__*/ [
    womanFarmer,
    womanLightSkinToneFarmer,
    womanMediumLightSkinToneFarmer,
    womanMediumSkinToneFarmer,
    womanMediumDarkSkinToneFarmer,
    womanDarkSkinToneFarmer
];

export const allWomanFarmerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F33E}\uDF3E", "Woman: Farmer", ...allWomanFarmer);

export const allSheafOfRice = /*@__PURE__*/ [
    sheafOfRice,
    allManFarmerGroup,
    allWomanFarmerGroup
];

export const allSheafOfRiceGroup = /*@__PURE__*/ new EmojiGroup("\u{1F33E}\uDF3E", "Sheaf of Rice", ...allSheafOfRice);

export const allManCook = /*@__PURE__*/ [
    manCook,
    manLightSkinToneCook,
    manMediumLightSkinToneCook,
    manMediumSkinToneCook,
    manMediumDarkSkinToneCook,
    manDarkSkinToneCook
];

export const allManCookGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F373}\uDF73", "Man: Cook", ...allManCook);

export const allWomanCook = /*@__PURE__*/ [
    womanCook,
    womanLightSkinToneCook,
    womanMediumLightSkinToneCook,
    womanMediumSkinToneCook,
    womanMediumDarkSkinToneCook,
    womanDarkSkinToneCook
];

export const allWomanCookGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F373}\uDF73", "Woman: Cook", ...allWomanCook);

export const allCooking = /*@__PURE__*/ [
    cooking,
    allManCookGroup,
    allWomanCookGroup
];

export const allCookingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F373}\uDF73", "Cooking", ...allCooking);

export const allManMechanic = /*@__PURE__*/ [
    manMechanic,
    manLightSkinToneMechanic,
    manMediumLightSkinToneMechanic,
    manMediumSkinToneMechanic,
    manMediumDarkSkinToneMechanic,
    manDarkSkinToneMechanic
];

export const allManMechanicGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F527}\uDD27", "Man: Mechanic", ...allManMechanic);

export const allWomanMechanic = /*@__PURE__*/ [
    womanMechanic,
    womanLightSkinToneMechanic,
    womanMediumLightSkinToneMechanic,
    womanMediumSkinToneMechanic,
    womanMediumDarkSkinToneMechanic,
    womanDarkSkinToneMechanic
];

export const allWomanMechanicGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F527}\uDD27", "Woman: Mechanic", ...allWomanMechanic);

export const allWrench = /*@__PURE__*/ [
    wrench,
    allManMechanicGroup,
    allWomanMechanicGroup
];

export const allWrenchGroup = /*@__PURE__*/ new EmojiGroup("\u{1F527}\uDD27", "Wrench", ...allWrench);

export const allManFactoryWorker = /*@__PURE__*/ [
    manFactoryWorker,
    manLightSkinToneFactoryWorker,
    manMediumLightSkinToneFactoryWorker,
    manMediumSkinToneFactoryWorker,
    manMediumDarkSkinToneFactoryWorker,
    manDarkSkinToneFactoryWorker
];

export const allManFactoryWorkerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F3ED}\uDFED", "Man: Factory Worker", ...allManFactoryWorker);

export const allWomanFactoryWorker = /*@__PURE__*/ [
    womanFactoryWorker,
    womanLightSkinToneFactoryWorker,
    womanMediumLightSkinToneFactoryWorker,
    womanMediumSkinToneFactoryWorker,
    womanMediumDarkSkinToneFactoryWorker,
    womanDarkSkinToneFactoryWorker
];

export const allWomanFactoryWorkerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F3ED}\uDFED", "Woman: Factory Worker", ...allWomanFactoryWorker);

export const allFactory = /*@__PURE__*/ [
    factory,
    allManFactoryWorkerGroup,
    allWomanFactoryWorkerGroup
];

export const allFactoryGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3ED}\uDFED", "Factory", ...allFactory);

export const allManOfficeWorker = /*@__PURE__*/ [
    manOfficeWorker,
    manLightSkinToneOfficeWorker,
    manMediumLightSkinToneOfficeWorker,
    manMediumSkinToneOfficeWorker,
    manMediumDarkSkinToneOfficeWorker,
    manDarkSkinToneOfficeWorker
];

export const allManOfficeWorkerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F4BC}\uDCBC", "Man: Office Worker", ...allManOfficeWorker);

export const allWomanOfficeWorker = /*@__PURE__*/ [
    womanOfficeWorker,
    womanLightSkinToneOfficeWorker,
    womanMediumLightSkinToneOfficeWorker,
    womanMediumSkinToneOfficeWorker,
    womanMediumDarkSkinToneOfficeWorker,
    womanDarkSkinToneOfficeWorker
];

export const allWomanOfficeWorkerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F4BC}\uDCBC", "Woman: Office Worker", ...allWomanOfficeWorker);

export const allBriefcase = /*@__PURE__*/ [
    briefcase,
    allManOfficeWorkerGroup,
    allWomanOfficeWorkerGroup
];

export const allBriefcaseGroup = /*@__PURE__*/ new EmojiGroup("\u{1F4BC}\uDCBC", "Briefcase", ...allBriefcase);

export const allManFireFighter = /*@__PURE__*/ [
    manFireFighter,
    manLightSkinToneFireFighter,
    manMediumLightSkinToneFireFighter,
    manMediumSkinToneFireFighter,
    manMediumDarkSkinToneFireFighter,
    manDarkSkinToneFireFighter
];

export const allManFireFighterGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F692}\uDE92", "Man: Fire Fighter", ...allManFireFighter);

export const allWomanFireFighter = /*@__PURE__*/ [
    womanFireFighter,
    womanLightSkinToneFireFighter,
    womanMediumLightSkinToneFireFighter,
    womanMediumSkinToneFireFighter,
    womanMediumDarkSkinToneFireFighter,
    womanDarkSkinToneFireFighter
];

export const allWomanFireFighterGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F692}\uDE92", "Woman: Fire Fighter", ...allWomanFireFighter);

export const allFireEngine = /*@__PURE__*/ [
    fireEngine,
    allManFireFighterGroup,
    allWomanFireFighterGroup
];

export const allFireEngineGroup = /*@__PURE__*/ new EmojiGroup("\u{1F692}\uDE92", "Fire Engine", ...allFireEngine);

export const allManAstronaut = /*@__PURE__*/ [
    manAstronaut,
    manLightSkinToneAstronaut,
    manMediumLightSkinToneAstronaut,
    manMediumSkinToneAstronaut,
    manMediumDarkSkinToneAstronaut,
    manDarkSkinToneAstronaut
];

export const allManAstronautGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F680}\uDE80", "Man: Astronaut", ...allManAstronaut);

export const allWomanAstronaut = /*@__PURE__*/ [
    womanAstronaut,
    womanLightSkinToneAstronaut,
    womanMediumLightSkinToneAstronaut,
    womanMediumSkinToneAstronaut,
    womanMediumDarkSkinToneAstronaut,
    womanDarkSkinToneAstronaut
];

export const allWomanAstronautGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F680}\uDE80", "Woman: Astronaut", ...allWomanAstronaut);

export const allRocket = /*@__PURE__*/ [
    rocket,
    allManAstronautGroup,
    allWomanAstronautGroup
];

export const allRocketGroup = /*@__PURE__*/ new EmojiGroup("\u{1F680}\uDE80", "Rocket", ...allRocket);

export const allManPilot = /*@__PURE__*/ [
    manPilot,
    manLightSkinTonePilot,
    manMediumLightSkinTonePilot,
    manMediumSkinTonePilot,
    manMediumDarkSkinTonePilot,
    manDarkSkinTonePilot
];

export const allManPilotGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u2708\uFE0F", "Man: Pilot", ...allManPilot);

export const allWomanPilot = /*@__PURE__*/ [
    womanPilot,
    womanLightSkinTonePilot,
    womanMediumLightSkinTonePilot,
    womanMediumSkinTonePilot,
    womanMediumDarkSkinTonePilot,
    womanDarkSkinTonePilot
];

export const allWomanPilotGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u2708\uFE0F", "Woman: Pilot", ...allWomanPilot);

export const allAirplane = /*@__PURE__*/ [
    airplane,
    allManPilotGroup,
    allWomanPilotGroup
];

export const allAirplaneGroup = /*@__PURE__*/ new EmojiGroup("\u2708\uFE0F", "Airplane", ...allAirplane);

export const allManArtist = /*@__PURE__*/ [
    manArtist,
    manLightSkinToneArtist,
    manMediumLightSkinToneArtist,
    manMediumSkinToneArtist,
    manMediumDarkSkinToneArtist,
    manDarkSkinToneArtist
];

export const allManArtistGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F3A8}\uDFA8", "Man: Artist", ...allManArtist);

export const allWomanArtist = /*@__PURE__*/ [
    womanArtist,
    womanLightSkinToneArtist,
    womanMediumLightSkinToneArtist,
    womanMediumSkinToneArtist,
    womanMediumDarkSkinToneArtist,
    womanDarkSkinToneArtist
];

export const allWomanArtistGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F3A8}\uDFA8", "Woman: Artist", ...allWomanArtist);

export const allArtistPalette = /*@__PURE__*/ [
    artistPalette,
    allManArtistGroup,
    allWomanArtistGroup
];

export const allArtistPaletteGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3A8}\uDFA8", "Artist Palette", ...allArtistPalette);

export const allManSinger = /*@__PURE__*/ [
    manSinger,
    manLightSkinToneSinger,
    manMediumLightSkinToneSinger,
    manMediumSkinToneSinger,
    manMediumDarkSkinToneSinger,
    manDarkSkinToneSinger
];

export const allManSingerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F3A4}\uDFA4", "Man: Singer", ...allManSinger);

export const allWomanSinger = /*@__PURE__*/ [
    womanSinger,
    womanLightSkinToneSinger,
    womanMediumLightSkinToneSinger,
    womanMediumSkinToneSinger,
    womanMediumDarkSkinToneSinger,
    womanDarkSkinToneSinger
];

export const allWomanSingerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F3A4}\uDFA4", "Woman: Singer", ...allWomanSinger);

export const allMicrophone = /*@__PURE__*/ [
    microphone,
    allManSingerGroup,
    allWomanSingerGroup
];

export const allMicrophoneGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3A4}\uDFA4", "Microphone", ...allMicrophone);

export const allManTechnologist = /*@__PURE__*/ [
    manTechnologist,
    manLightSkinToneTechnologist,
    manMediumLightSkinToneTechnologist,
    manMediumSkinToneTechnologist,
    manMediumDarkSkinToneTechnologist,
    manDarkSkinToneTechnologist
];

export const allManTechnologistGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F4BB}\uDCBB", "Man: Technologist", ...allManTechnologist);

export const allWomanTechnologist = /*@__PURE__*/ [
    womanTechnologist,
    womanLightSkinToneTechnologist,
    womanMediumLightSkinToneTechnologist,
    womanMediumSkinToneTechnologist,
    womanMediumDarkSkinToneTechnologist,
    womanDarkSkinToneTechnologist
];

export const allWomanTechnologistGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F4BB}\uDCBB", "Woman: Technologist", ...allWomanTechnologist);

export const allLaptop = /*@__PURE__*/ [
    laptop,
    allManTechnologistGroup,
    allWomanTechnologistGroup
];

export const allLaptopGroup = /*@__PURE__*/ new EmojiGroup("\u{1F4BB}\uDCBB", "Laptop", ...allLaptop);

export const allManScientist = /*@__PURE__*/ [
    manScientist,
    manLightSkinToneScientist,
    manMediumLightSkinToneScientist,
    manMediumSkinToneScientist,
    manMediumDarkSkinToneScientist,
    manDarkSkinToneScientist
];

export const allManScientistGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F52C}\uDD2C", "Man: Scientist", ...allManScientist);

export const allWomanScientist = /*@__PURE__*/ [
    womanScientist,
    womanLightSkinToneScientist,
    womanMediumLightSkinToneScientist,
    womanMediumSkinToneScientist,
    womanMediumDarkSkinToneScientist,
    womanDarkSkinToneScientist
];

export const allWomanScientistGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F52C}\uDD2C", "Woman: Scientist", ...allWomanScientist);

export const allMicroscope = /*@__PURE__*/ [
    microscope,
    allManScientistGroup,
    allWomanScientistGroup
];

export const allMicroscopeGroup = /*@__PURE__*/ new EmojiGroup("\u{1F52C}\uDD2C", "Microscope", ...allMicroscope);

export const allPrince = /*@__PURE__*/ [
    prince,
    princeLightSkinTone,
    princeMediumLightSkinTone,
    princeMediumSkinTone,
    princeMediumDarkSkinTone,
    princeDarkSkinTone
];

export const allPrinceGroup = /*@__PURE__*/ new EmojiGroup("\u{1F934}\uDD34", "Prince", ...allPrince);

export const allPrincess = /*@__PURE__*/ [
    princess,
    princessLightSkinTone,
    princessMediumLightSkinTone,
    princessMediumSkinTone,
    princessMediumDarkSkinTone,
    princessDarkSkinTone
];

export const allPrincessGroup = /*@__PURE__*/ new EmojiGroup("\u{1F478}\uDC78", "Princess", ...allPrincess);

export const allRoyalty = /*@__PURE__*/ [
    crown,
    allPrinceGroup,
    allPrincessGroup
];

export const allRoyaltyGroup = /*@__PURE__*/ new EmojiGroup("\u{1F451}\uDC51", "Crown", ...allRoyalty);

export const allOccupation = /*@__PURE__*/ [
    allMedicalGroup,
    allGraduationCapGroup,
    allSchoolGroup,
    allBalanceScaleGroup,
    allSheafOfRiceGroup,
    allCookingGroup,
    allWrenchGroup,
    allFactoryGroup,
    allBriefcaseGroup,
    allMicroscopeGroup,
    allLaptopGroup,
    allMicrophoneGroup,
    allArtistPaletteGroup,
    allAirplaneGroup,
    allRocketGroup,
    allFireEngineGroup,
    allSpiesGroup,
    allGuardsGroup,
    allConstructionWorkersGroup,
    allRoyaltyGroup
];

export const allOccupationGroup = /*@__PURE__*/ new EmojiGroup("\u0052\u006F\u006C\u0065\u0073", "Depictions of people working", ...allOccupation);

export const allCherub = /*@__PURE__*/ [
    cherub,
    cherubLightSkinTone,
    cherubMediumLightSkinTone,
    cherubMediumSkinTone,
    cherubMediumDarkSkinTone,
    cherubDarkSkinTone
];

export const allCherubGroup = /*@__PURE__*/ new EmojiGroup("\u{1F47C}\uDC7C", "Cherub", ...allCherub);

export const allSantaClaus = /*@__PURE__*/ [
    santaClaus,
    santaClausLightSkinTone,
    santaClausMediumLightSkinTone,
    santaClausMediumSkinTone,
    santaClausMediumDarkSkinTone,
    santaClausDarkSkinTone
];

export const allSantaClausGroup = /*@__PURE__*/ new EmojiGroup("\u{1F385}\uDF85", "Santa Claus", ...allSantaClaus);

export const allMrsClaus = /*@__PURE__*/ [
    mrsClaus,
    mrsClausLightSkinTone,
    mrsClausMediumLightSkinTone,
    mrsClausMediumSkinTone,
    mrsClausMediumDarkSkinTone,
    mrsClausDarkSkinTone
];

export const allMrsClausGroup = /*@__PURE__*/ new EmojiGroup("\u{1F936}\uDD36", "Mrs. Claus", ...allMrsClaus);

export const allGenie = /*@__PURE__*/ [
    genie,
    genieMale,
    genieFemale
];

export const allGenieGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DE}\uDDDE", "Genie", ...allGenie);

export const allZombie = /*@__PURE__*/ [
    zombie,
    zombieMale,
    zombieFemale
];

export const allZombieGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9DF}\uDDDF", "Zombie", ...allZombie);

export const allFantasy = /*@__PURE__*/ [
    allCherubGroup,
    allSantaClausGroup,
    allMrsClausGroup,
    allSuperheroesGroup,
    allSupervillainsGroup,
    allMagesGroup,
    allFairiesGroup,
    allVampiresGroup,
    allMerpeopleGroup,
    allElvesGroup,
    allGenieGroup,
    allZombieGroup
];

export const allFantasyGroup = /*@__PURE__*/ new EmojiGroup("\u0046\u0061\u006E\u0074\u0061\u0073\u0079", "Depictions of fantasy characters", ...allFantasy);

export const allManProbing = /*@__PURE__*/ [
    manProbing,
    manLightSkinToneProbing,
    manMediumLightSkinToneProbing,
    manMediumSkinToneProbing,
    manMediumDarkSkinToneProbing,
    manDarkSkinToneProbing
];

export const allManProbingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9AF}\uDDAF", "Man: Probing", ...allManProbing);

export const allWomanProbing = /*@__PURE__*/ [
    womanProbing,
    womanLightSkinToneProbing,
    womanMediumLightSkinToneProbing,
    womanMediumSkinToneProbing,
    womanMediumDarkSkinToneProbing,
    womanDarkSkinToneProbing
];

export const allWomanProbingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9AF}\uDDAF", "Woman: Probing", ...allWomanProbing);

export const allProbingCane = /*@__PURE__*/ [
    probingCane,
    allManProbingGroup,
    allWomanProbingGroup
];

export const allProbingCaneGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9AF}\uDDAF", "Probing Cane", ...allProbingCane);

export const allManInMotorizedWheelchair = /*@__PURE__*/ [
    manInMotorizedWheelchair,
    manLightSkinToneInMotorizedWheelchair,
    manMediumLightSkinToneInMotorizedWheelchair,
    manMediumSkinToneInMotorizedWheelchair,
    manMediumDarkSkinToneInMotorizedWheelchair,
    manDarkSkinToneInMotorizedWheelchair
];

export const allManInMotorizedWheelchairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9BC}\uDDBC", "Man: In Motorized Wheelchair", ...allManInMotorizedWheelchair);

export const allWomanInMotorizedWheelchair = /*@__PURE__*/ [
    womanInMotorizedWheelchair,
    womanLightSkinToneInMotorizedWheelchair,
    womanMediumLightSkinToneInMotorizedWheelchair,
    womanMediumSkinToneInMotorizedWheelchair,
    womanMediumDarkSkinToneInMotorizedWheelchair,
    womanDarkSkinToneInMotorizedWheelchair
];

export const allWomanInMotorizedWheelchairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9BC}\uDDBC", "Woman: In Motorized Wheelchair", ...allWomanInMotorizedWheelchair);

export const allMotorizedWheelchair = /*@__PURE__*/ [
    motorizedWheelchair,
    allManInMotorizedWheelchairGroup,
    allWomanInMotorizedWheelchairGroup
];

export const allMotorizedWheelchairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9BC}\uDDBC", "Motorized Wheelchair", ...allMotorizedWheelchair);

export const allManInManualWheelchair = /*@__PURE__*/ [
    manInManualWheelchair,
    manLightSkinToneInManualWheelchair,
    manMediumLightSkinToneInManualWheelchair,
    manMediumSkinToneInManualWheelchair,
    manMediumDarkSkinToneInManualWheelchair,
    manDarkSkinToneInManualWheelchair
];

export const allManInManualWheelchairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F468}\uDC68\u200D\u{1F9BD}\uDDBD", "Man: In Manual Wheelchair", ...allManInManualWheelchair);

export const allWomanInManualWheelchair = /*@__PURE__*/ [
    womanInManualWheelchair,
    womanLightSkinToneInManualWheelchair,
    womanMediumLightSkinToneInManualWheelchair,
    womanMediumSkinToneInManualWheelchair,
    womanMediumDarkSkinToneInManualWheelchair,
    womanDarkSkinToneInManualWheelchair
];

export const allWomanInManualWheelchairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F469}\uDC69\u200D\u{1F9BD}\uDDBD", "Woman: In Manual Wheelchair", ...allWomanInManualWheelchair);

export const allManualWheelchair = /*@__PURE__*/ [
    manualWheelchair,
    allManInManualWheelchairGroup,
    allWomanInManualWheelchairGroup
];

export const allManualWheelchairGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9BD}\uDDBD", "Manual Wheelchair", ...allManualWheelchair);

export const allManDancing = /*@__PURE__*/ [
    manDancing,
    manDancingLightSkinTone,
    manDancingMediumLightSkinTone,
    manDancingMediumSkinTone,
    manDancingMediumDarkSkinTone,
    manDancingDarkSkinTone
];

export const allManDancingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F57A}\uDD7A", "Man Dancing", ...allManDancing);

export const allWomanDancing = /*@__PURE__*/ [
    womanDancing,
    womanDancingLightSkinTone,
    womanDancingMediumLightSkinTone,
    womanDancingMediumSkinTone,
    womanDancingMediumDarkSkinTone,
    womanDancingDarkSkinTone
];

export const allWomanDancingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F483}\uDC83", "Woman Dancing", ...allWomanDancing);

export const allMenDancing = /*@__PURE__*/ [
    allManDancingGroup,
    allWomanDancingGroup
];

export const allMenDancingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F57A}\uDD7A", "Dancing", ...allMenDancing);

export const allJuggler = /*@__PURE__*/ [
    juggler,
    jugglerLightSkinTone,
    jugglerMediumLightSkinTone,
    jugglerMediumSkinTone,
    jugglerMediumDarkSkinTone,
    jugglerDarkSkinTone
];

export const allJugglerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F939}\uDD39", "Juggler", ...allJuggler);

export const allJugglerMale = /*@__PURE__*/ [
    jugglerMale,
    jugglerLightSkinToneMale,
    jugglerMediumLightSkinToneMale,
    jugglerMediumSkinToneMale,
    jugglerMediumDarkSkinToneMale,
    jugglerDarkSkinToneMale
];

export const allJugglerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F939}\uDD39\u200D\u2642\uFE0F", "Juggler: Male", ...allJugglerMale);

export const allJugglerFemale = /*@__PURE__*/ [
    jugglerFemale,
    jugglerLightSkinToneFemale,
    jugglerMediumLightSkinToneFemale,
    jugglerMediumSkinToneFemale,
    jugglerMediumDarkSkinToneFemale,
    jugglerDarkSkinToneFemale
];

export const allJugglerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F939}\uDD39\u200D\u2640\uFE0F", "Juggler: Female", ...allJugglerFemale);

export const allJugglers = /*@__PURE__*/ [
    allJugglerGroup,
    allJugglerMaleGroup,
    allJugglerFemaleGroup
];

export const allJugglersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F939}\uDD39", "Juggler", ...allJugglers);

export const allClimber = /*@__PURE__*/ [
    climber,
    climberLightSkinTone,
    climberMediumLightSkinTone,
    climberMediumSkinTone,
    climberMediumDarkSkinTone,
    climberDarkSkinTone
];

export const allClimberGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D7}\uDDD7", "Climber", ...allClimber);

export const allClimberMale = /*@__PURE__*/ [
    climberMale,
    climberLightSkinToneMale,
    climberMediumLightSkinToneMale,
    climberMediumSkinToneMale,
    climberMediumDarkSkinToneMale,
    climberDarkSkinToneMale
];

export const allClimberMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D7}\uDDD7\u200D\u2642\uFE0F", "Climber: Male", ...allClimberMale);

export const allClimberFemale = /*@__PURE__*/ [
    climberFemale,
    climberLightSkinToneFemale,
    climberMediumLightSkinToneFemale,
    climberMediumSkinToneFemale,
    climberMediumDarkSkinToneFemale,
    climberDarkSkinToneFemale
];

export const allClimberFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D7}\uDDD7\u200D\u2640\uFE0F", "Climber: Female", ...allClimberFemale);

export const allClimbers = /*@__PURE__*/ [
    allClimberGroup,
    allClimberMaleGroup,
    allClimberFemaleGroup
];

export const allClimbersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D7}\uDDD7", "Climber", ...allClimbers);

export const allJockey = /*@__PURE__*/ [
    jockey,
    jockeyLightSkinTone,
    jockeyMediumLightSkinTone,
    jockeyMediumSkinTone,
    jockeyMediumDarkSkinTone,
    jockeyDarkSkinTone
];

export const allJockeyGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C7}\uDFC7", "Jockey", ...allJockey);

export const allSnowboarder = /*@__PURE__*/ [
    snowboarder,
    snowboarderLightSkinTone,
    snowboarderMediumLightSkinTone,
    snowboarderMediumSkinTone,
    snowboarderMediumDarkSkinTone,
    snowboarderDarkSkinTone
];

export const allSnowboarderGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C2}\uDFC2", "Snowboarder", ...allSnowboarder);

export const allGolfer = /*@__PURE__*/ [
    golfer,
    golferLightSkinTone,
    golferMediumLightSkinTone,
    golferMediumSkinTone,
    golferMediumDarkSkinTone,
    golferDarkSkinTone
];

export const allGolferGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CC}\uDFCC\uFE0F", "Golfer", ...allGolfer);

export const allGolferMale = /*@__PURE__*/ [
    golferMale,
    golferLightSkinToneMale,
    golferMediumLightSkinToneMale,
    golferMediumSkinToneMale,
    golferMediumDarkSkinToneMale,
    golferDarkSkinToneMale
];

export const allGolferMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CC}\uDFCC\uFE0F\u200D\u2642\uFE0F", "Golfer: Male", ...allGolferMale);

export const allGolferFemale = /*@__PURE__*/ [
    golferFemale,
    golferLightSkinToneFemale,
    golferMediumLightSkinToneFemale,
    golferMediumSkinToneFemale,
    golferMediumDarkSkinToneFemale,
    golferDarkSkinToneFemale
];

export const allGolferFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CC}\uDFCC\uFE0F\u200D\u2640\uFE0F", "Golfer: Female", ...allGolferFemale);

export const allGolfers = /*@__PURE__*/ [
    allGolferGroup,
    allGolferMaleGroup,
    allGolferFemaleGroup
];

export const allGolfersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CC}\uDFCC\uFE0F", "Golfer", ...allGolfers);

export const allSurfing = /*@__PURE__*/ [
    surfing,
    surfingLightSkinTone,
    surfingMediumLightSkinTone,
    surfingMediumSkinTone,
    surfingMediumDarkSkinTone,
    surfingDarkSkinTone
];

export const allSurfingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C4}\uDFC4", "Surfing", ...allSurfing);

export const allSurfingMale = /*@__PURE__*/ [
    surfingMale,
    surfingLightSkinToneMale,
    surfingMediumLightSkinToneMale,
    surfingMediumSkinToneMale,
    surfingMediumDarkSkinToneMale,
    surfingDarkSkinToneMale
];

export const allSurfingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C4}\uDFC4\u200D\u2642\uFE0F", "Surfing: Male", ...allSurfingMale);

export const allSurfingFemale = /*@__PURE__*/ [
    surfingFemale,
    surfingLightSkinToneFemale,
    surfingMediumLightSkinToneFemale,
    surfingMediumSkinToneFemale,
    surfingMediumDarkSkinToneFemale,
    surfingDarkSkinToneFemale
];

export const allSurfingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C4}\uDFC4\u200D\u2640\uFE0F", "Surfing: Female", ...allSurfingFemale);

export const allSurfers = /*@__PURE__*/ [
    allSurfingGroup,
    allSurfingMaleGroup,
    allSurfingFemaleGroup
];

export const allSurfersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3C4}\uDFC4", "Surfing", ...allSurfers);

export const allRowingBoat = /*@__PURE__*/ [
    rowingBoat,
    rowingBoatLightSkinTone,
    rowingBoatMediumLightSkinTone,
    rowingBoatMediumSkinTone,
    rowingBoatMediumDarkSkinTone,
    rowingBoatDarkSkinTone
];

export const allRowingBoatGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6A3}\uDEA3", "Rowing Boat", ...allRowingBoat);

export const allRowingBoatMale = /*@__PURE__*/ [
    rowingBoatMale,
    rowingBoatLightSkinToneMale,
    rowingBoatMediumLightSkinToneMale,
    rowingBoatMediumSkinToneMale,
    rowingBoatMediumDarkSkinToneMale,
    rowingBoatDarkSkinToneMale
];

export const allRowingBoatMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6A3}\uDEA3\u200D\u2642\uFE0F", "Rowing Boat: Male", ...allRowingBoatMale);

export const allRowingBoatFemale = /*@__PURE__*/ [
    rowingBoatFemale,
    rowingBoatLightSkinToneFemale,
    rowingBoatMediumLightSkinToneFemale,
    rowingBoatMediumSkinToneFemale,
    rowingBoatMediumDarkSkinToneFemale,
    rowingBoatDarkSkinToneFemale
];

export const allRowingBoatFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6A3}\uDEA3\u200D\u2640\uFE0F", "Rowing Boat: Female", ...allRowingBoatFemale);

export const allBoatRowers = /*@__PURE__*/ [
    allRowingBoatGroup,
    allRowingBoatMaleGroup,
    allRowingBoatFemaleGroup
];

export const allBoatRowersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6A3}\uDEA3", "Rowing Boat", ...allBoatRowers);

export const allSwimming = /*@__PURE__*/ [
    swimming,
    swimmingLightSkinTone,
    swimmingMediumLightSkinTone,
    swimmingMediumSkinTone,
    swimmingMediumDarkSkinTone,
    swimmingDarkSkinTone
];

export const allSwimmingGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CA}\uDFCA", "Swimming", ...allSwimming);

export const allSwimmingMale = /*@__PURE__*/ [
    swimmingMale,
    swimmingLightSkinToneMale,
    swimmingMediumLightSkinToneMale,
    swimmingMediumSkinToneMale,
    swimmingMediumDarkSkinToneMale,
    swimmingDarkSkinToneMale
];

export const allSwimmingMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CA}\uDFCA\u200D\u2642\uFE0F", "Swimming: Male", ...allSwimmingMale);

export const allSwimmingFemale = /*@__PURE__*/ [
    swimmingFemale,
    swimmingLightSkinToneFemale,
    swimmingMediumLightSkinToneFemale,
    swimmingMediumSkinToneFemale,
    swimmingMediumDarkSkinToneFemale,
    swimmingDarkSkinToneFemale
];

export const allSwimmingFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CA}\uDFCA\u200D\u2640\uFE0F", "Swimming: Female", ...allSwimmingFemale);

export const allSwimmers = /*@__PURE__*/ [
    allSwimmingGroup,
    allSwimmingMaleGroup,
    allSwimmingFemaleGroup
];

export const allSwimmersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CA}\uDFCA", "Swimming", ...allSwimmers);

export const allBasketBaller = /*@__PURE__*/ [
    basketBaller,
    basketBallerLightSkinTone,
    basketBallerMediumLightSkinTone,
    basketBallerMediumSkinTone,
    basketBallerMediumDarkSkinTone,
    basketBallerDarkSkinTone
];

export const allBasketBallerGroup = /*@__PURE__*/ new EmojiGroup("\u26F9\uFE0F", "Basket Baller", ...allBasketBaller);

export const allBasketBallerMale = /*@__PURE__*/ [
    basketBallerMale,
    basketBallerLightSkinToneMale,
    basketBallerMediumLightSkinToneMale,
    basketBallerMediumSkinToneMale,
    basketBallerMediumDarkSkinToneMale,
    basketBallerDarkSkinToneMale
];

export const allBasketBallerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u26F9\uFE0F\u200D\u2642\uFE0F", "Basket Baller: Male", ...allBasketBallerMale);

export const allBasketBallerFemale = /*@__PURE__*/ [
    basketBallerFemale,
    basketBallerLightSkinToneFemale,
    basketBallerMediumLightSkinToneFemale,
    basketBallerMediumSkinToneFemale,
    basketBallerMediumDarkSkinToneFemale,
    basketBallerDarkSkinToneFemale
];

export const allBasketBallerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u26F9\uFE0F\u200D\u2640\uFE0F", "Basket Baller: Female", ...allBasketBallerFemale);

export const allBacketBallPlayers = /*@__PURE__*/ [
    allBasketBallerGroup,
    allBasketBallerMaleGroup,
    allBasketBallerFemaleGroup
];

export const allBacketBallPlayersGroup = /*@__PURE__*/ new EmojiGroup("\u26F9\uFE0F", "Basket Baller", ...allBacketBallPlayers);

export const allWeightLifter = /*@__PURE__*/ [
    weightLifter,
    weightLifterLightSkinTone,
    weightLifterMediumLightSkinTone,
    weightLifterMediumSkinTone,
    weightLifterMediumDarkSkinTone,
    weightLifterDarkSkinTone
];

export const allWeightLifterGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CB}\uDFCB\uFE0F", "Weight Lifter", ...allWeightLifter);

export const allWeightLifterMale = /*@__PURE__*/ [
    weightLifterMale,
    weightLifterLightSkinToneMale,
    weightLifterMediumLightSkinToneMale,
    weightLifterMediumSkinToneMale,
    weightLifterMediumDarkSkinToneMale,
    weightLifterDarkSkinToneMale
];

export const allWeightLifterMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CB}\uDFCB\uFE0F\u200D\u2642\uFE0F", "Weight Lifter: Male", ...allWeightLifterMale);

export const allWeightLifterFemale = /*@__PURE__*/ [
    weightLifterFemale,
    weightLifterLightSkinToneFemale,
    weightLifterMediumLightSkinToneFemale,
    weightLifterMediumSkinToneFemale,
    weightLifterMediumDarkSkinToneFemale,
    weightLifterDarkSkinToneFemale
];

export const allWeightLifterFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CB}\uDFCB\uFE0F\u200D\u2640\uFE0F", "Weight Lifter: Female", ...allWeightLifterFemale);

export const allWeightLifters = /*@__PURE__*/ [
    allWeightLifterGroup,
    allWeightLifterMaleGroup,
    allWeightLifterFemaleGroup
];

export const allWeightLiftersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F3CB}\uDFCB\uFE0F", "Weight Lifter", ...allWeightLifters);

export const allBiker = /*@__PURE__*/ [
    biker,
    bikerLightSkinTone,
    bikerMediumLightSkinTone,
    bikerMediumSkinTone,
    bikerMediumDarkSkinTone,
    bikerDarkSkinTone
];

export const allBikerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B4}\uDEB4", "Biker", ...allBiker);

export const allBikerMale = /*@__PURE__*/ [
    bikerMale,
    bikerLightSkinToneMale,
    bikerMediumLightSkinToneMale,
    bikerMediumSkinToneMale,
    bikerMediumDarkSkinToneMale,
    bikerDarkSkinToneMale
];

export const allBikerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B4}\uDEB4\u200D\u2642\uFE0F", "Biker: Male", ...allBikerMale);

export const allBikerFemale = /*@__PURE__*/ [
    bikerFemale,
    bikerLightSkinToneFemale,
    bikerMediumLightSkinToneFemale,
    bikerMediumSkinToneFemale,
    bikerMediumDarkSkinToneFemale,
    bikerDarkSkinToneFemale
];

export const allBikerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B4}\uDEB4\u200D\u2640\uFE0F", "Biker: Female", ...allBikerFemale);

export const allBikers = /*@__PURE__*/ [
    allBikerGroup,
    allBikerMaleGroup,
    allBikerFemaleGroup
];

export const allBikersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B4}\uDEB4", "Biker", ...allBikers);

export const allMountainBiker = /*@__PURE__*/ [
    mountainBiker,
    mountainBikerLightSkinTone,
    mountainBikerMediumLightSkinTone,
    mountainBikerMediumSkinTone,
    mountainBikerMediumDarkSkinTone,
    mountainBikerDarkSkinTone
];

export const allMountainBikerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B5}\uDEB5", "Mountain Biker", ...allMountainBiker);

export const allMountainBikerMale = /*@__PURE__*/ [
    mountainBikerMale,
    mountainBikerLightSkinToneMale,
    mountainBikerMediumLightSkinToneMale,
    mountainBikerMediumSkinToneMale,
    mountainBikerMediumDarkSkinToneMale,
    mountainBikerDarkSkinToneMale
];

export const allMountainBikerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B5}\uDEB5\u200D\u2642\uFE0F", "Mountain Biker: Male", ...allMountainBikerMale);

export const allMountainBikerFemale = /*@__PURE__*/ [
    mountainBikerFemale,
    mountainBikerLightSkinToneFemale,
    mountainBikerMediumLightSkinToneFemale,
    mountainBikerMediumSkinToneFemale,
    mountainBikerMediumDarkSkinToneFemale,
    mountainBikerDarkSkinToneFemale
];

export const allMountainBikerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B5}\uDEB5\u200D\u2640\uFE0F", "Mountain Biker: Female", ...allMountainBikerFemale);

export const allMountainBikers = /*@__PURE__*/ [
    allMountainBikerGroup,
    allMountainBikerMaleGroup,
    allMountainBikerFemaleGroup
];

export const allMountainBikersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6B5}\uDEB5", "Mountain Biker", ...allMountainBikers);

export const allCartwheeler = /*@__PURE__*/ [
    cartwheeler,
    cartwheelerLightSkinTone,
    cartwheelerMediumLightSkinTone,
    cartwheelerMediumSkinTone,
    cartwheelerMediumDarkSkinTone,
    cartwheelerDarkSkinTone
];

export const allCartwheelerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F938}\uDD38", "Cartwheeler", ...allCartwheeler);

export const allCartwheelerMale = /*@__PURE__*/ [
    cartwheelerMale,
    cartwheelerLightSkinToneMale,
    cartwheelerMediumLightSkinToneMale,
    cartwheelerMediumSkinToneMale,
    cartwheelerMediumDarkSkinToneMale,
    cartwheelerDarkSkinToneMale
];

export const allCartwheelerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F938}\uDD38\u200D\u2642\uFE0F", "Cartwheeler: Male", ...allCartwheelerMale);

export const allCartwheelerFemale = /*@__PURE__*/ [
    cartwheelerFemale,
    cartwheelerLightSkinToneFemale,
    cartwheelerMediumLightSkinToneFemale,
    cartwheelerMediumSkinToneFemale,
    cartwheelerMediumDarkSkinToneFemale,
    cartwheelerDarkSkinToneFemale
];

export const allCartwheelerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F938}\uDD38\u200D\u2640\uFE0F", "Cartwheeler: Female", ...allCartwheelerFemale);

export const allCartwheelers = /*@__PURE__*/ [
    allCartwheelerGroup,
    allCartwheelerMaleGroup,
    allCartwheelerFemaleGroup
];

export const allCartwheelersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F938}\uDD38", "Cartwheeler", ...allCartwheelers);

export const allWrestler = /*@__PURE__*/ [
    wrestler,
    wrestlerMale,
    wrestlerFemale
];

export const allWrestlerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93C}\uDD3C", "Wrestler", ...allWrestler);

export const allWaterPoloPlayer = /*@__PURE__*/ [
    waterPoloPlayer,
    waterPoloPlayerLightSkinTone,
    waterPoloPlayerMediumLightSkinTone,
    waterPoloPlayerMediumSkinTone,
    waterPoloPlayerMediumDarkSkinTone,
    waterPoloPlayerDarkSkinTone
];

export const allWaterPoloPlayerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93D}\uDD3D", "Water Polo Player", ...allWaterPoloPlayer);

export const allWaterPoloPlayerMale = /*@__PURE__*/ [
    waterPoloPlayerMale,
    waterPoloPlayerLightSkinToneMale,
    waterPoloPlayerMediumLightSkinToneMale,
    waterPoloPlayerMediumSkinToneMale,
    waterPoloPlayerMediumDarkSkinToneMale,
    waterPoloPlayerDarkSkinToneMale
];

export const allWaterPoloPlayerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93D}\uDD3D\u200D\u2642\uFE0F", "Water Polo Player: Male", ...allWaterPoloPlayerMale);

export const allWaterPoloPlayerFemale = /*@__PURE__*/ [
    waterPoloPlayerFemale,
    waterPoloPlayerLightSkinToneFemale,
    waterPoloPlayerMediumLightSkinToneFemale,
    waterPoloPlayerMediumSkinToneFemale,
    waterPoloPlayerMediumDarkSkinToneFemale,
    waterPoloPlayerDarkSkinToneFemale
];

export const allWaterPoloPlayerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93D}\uDD3D\u200D\u2640\uFE0F", "Water Polo Player: Female", ...allWaterPoloPlayerFemale);

export const allWaterPoloPlayers = /*@__PURE__*/ [
    allWaterPoloPlayerGroup,
    allWaterPoloPlayerMaleGroup,
    allWaterPoloPlayerFemaleGroup
];

export const allWaterPoloPlayersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93D}\uDD3D", "Water Polo Player", ...allWaterPoloPlayers);

export const allHandBaller = /*@__PURE__*/ [
    handBaller,
    handBallerLightSkinTone,
    handBallerMediumLightSkinTone,
    handBallerMediumSkinTone,
    handBallerMediumDarkSkinTone,
    handBallerDarkSkinTone
];

export const allHandBallerGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93E}\uDD3E", "Hand Baller", ...allHandBaller);

export const allHandBallerMale = /*@__PURE__*/ [
    handBallerMale,
    handBallerLightSkinToneMale,
    handBallerMediumLightSkinToneMale,
    handBallerMediumSkinToneMale,
    handBallerMediumDarkSkinToneMale,
    handBallerDarkSkinToneMale
];

export const allHandBallerMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93E}\uDD3E\u200D\u2642\uFE0F", "Hand Baller: Male", ...allHandBallerMale);

export const allHandBallerFemale = /*@__PURE__*/ [
    handBallerFemale,
    handBallerLightSkinToneFemale,
    handBallerMediumLightSkinToneFemale,
    handBallerMediumSkinToneFemale,
    handBallerMediumDarkSkinToneFemale,
    handBallerDarkSkinToneFemale
];

export const allHandBallerFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93E}\uDD3E\u200D\u2640\uFE0F", "Hand Baller: Female", ...allHandBallerFemale);

export const allHandBallers = /*@__PURE__*/ [
    allHandBallerGroup,
    allHandBallerMaleGroup,
    allHandBallerFemaleGroup
];

export const allHandBallersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F93E}\uDD3E", "Hand Baller", ...allHandBallers);

export const allInMotion = /*@__PURE__*/ [
    allWalkersGroup,
    allStandersGroup,
    allKneelersGroup,
    allProbingCaneGroup,
    allMotorizedWheelchairGroup,
    allManualWheelchairGroup,
    allMenDancingGroup,
    allJugglersGroup,
    allClimbersGroup,
    fencer,
    allJockeyGroup,
    skier,
    allSnowboarderGroup,
    allGolfersGroup,
    allSurfersGroup,
    allBoatRowersGroup,
    allSwimmersGroup,
    allRunnersGroup,
    allBacketBallPlayersGroup,
    allWeightLiftersGroup,
    allBikersGroup,
    allMountainBikersGroup,
    allCartwheelersGroup,
    allWrestlerGroup,
    allWaterPoloPlayersGroup,
    allHandBallersGroup
];

export const allInMotionGroup = /*@__PURE__*/ new EmojiGroup("\u0049\u006E\u0020\u004D\u006F\u0074\u0069\u006F\u006E", "Depictions of people in motion", ...allInMotion);

export const allInLotusPosition = /*@__PURE__*/ [
    inLotusPosition,
    inLotusPositionLightSkinTone,
    inLotusPositionMediumLightSkinTone,
    inLotusPositionMediumSkinTone,
    inLotusPositionMediumDarkSkinTone,
    inLotusPositionDarkSkinTone
];

export const allInLotusPositionGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D8}\uDDD8", "In Lotus Position", ...allInLotusPosition);

export const allInLotusPositionMale = /*@__PURE__*/ [
    inLotusPositionMale,
    inLotusPositionLightSkinToneMale,
    inLotusPositionMediumLightSkinToneMale,
    inLotusPositionMediumSkinToneMale,
    inLotusPositionMediumDarkSkinToneMale,
    inLotusPositionDarkSkinToneMale
];

export const allInLotusPositionMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D8}\uDDD8\u200D\u2642\uFE0F", "In Lotus Position: Male", ...allInLotusPositionMale);

export const allInLotusPositionFemale = /*@__PURE__*/ [
    inLotusPositionFemale,
    inLotusPositionLightSkinToneFemale,
    inLotusPositionMediumLightSkinToneFemale,
    inLotusPositionMediumSkinToneFemale,
    inLotusPositionMediumDarkSkinToneFemale,
    inLotusPositionDarkSkinToneFemale
];

export const allInLotusPositionFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D8}\uDDD8\u200D\u2640\uFE0F", "In Lotus Position: Female", ...allInLotusPositionFemale);

export const allLotusSitters = /*@__PURE__*/ [
    allInLotusPositionGroup,
    allInLotusPositionMaleGroup,
    allInLotusPositionFemaleGroup
];

export const allLotusSittersGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D8}\uDDD8", "In Lotus Position", ...allLotusSitters);

export const allInBath = /*@__PURE__*/ [
    inBath,
    inBathLightSkinTone,
    inBathMediumLightSkinTone,
    inBathMediumSkinTone,
    inBathMediumDarkSkinTone,
    inBathDarkSkinTone
];

export const allInBathGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6C0}\uDEC0", "In Bath", ...allInBath);

export const allInBed = /*@__PURE__*/ [
    inBed,
    inBedLightSkinTone,
    inBedMediumLightSkinTone,
    inBedMediumSkinTone,
    inBedMediumDarkSkinTone,
    inBedDarkSkinTone
];

export const allInBedGroup = /*@__PURE__*/ new EmojiGroup("\u{1F6CC}\uDECC", "In Bed", ...allInBed);

export const allInSauna = /*@__PURE__*/ [
    inSauna,
    inSaunaLightSkinTone,
    inSaunaMediumLightSkinTone,
    inSaunaMediumSkinTone,
    inSaunaMediumDarkSkinTone,
    inSaunaDarkSkinTone
];

export const allInSaunaGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D6}\uDDD6", "In Sauna", ...allInSauna);

export const allInSaunaMale = /*@__PURE__*/ [
    inSaunaMale,
    inSaunaLightSkinToneMale,
    inSaunaMediumLightSkinToneMale,
    inSaunaMediumSkinToneMale,
    inSaunaMediumDarkSkinToneMale,
    inSaunaDarkSkinToneMale
];

export const allInSaunaMaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D6}\uDDD6\u200D\u2642\uFE0F", "In Sauna: Male", ...allInSaunaMale);

export const allInSaunaFemale = /*@__PURE__*/ [
    inSaunaFemale,
    inSaunaLightSkinToneFemale,
    inSaunaMediumLightSkinToneFemale,
    inSaunaMediumSkinToneFemale,
    inSaunaMediumDarkSkinToneFemale,
    inSaunaDarkSkinToneFemale
];

export const allInSaunaFemaleGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D6}\uDDD6\u200D\u2640\uFE0F", "In Sauna: Female", ...allInSaunaFemale);

export const allSauna = /*@__PURE__*/ [
    allInSaunaGroup,
    allInSaunaMaleGroup,
    allInSaunaFemaleGroup
];

export const allSaunaGroup = /*@__PURE__*/ new EmojiGroup("\u{1F9D6}\uDDD6", "In Sauna", ...allSauna);

export const allResting = /*@__PURE__*/ [
    allLotusSittersGroup,
    allInBathGroup,
    allInBedGroup,
    allSaunaGroup
];

export const allRestingGroup = /*@__PURE__*/ new EmojiGroup("\u0052\u0065\u0073\u0074\u0069\u006E\u0067", "Depictions of people at rest", ...allResting);

export const allBabies = /*@__PURE__*/ [
    allBabyGroup,
    allCherubGroup
];

export const allBabiesGroup = /*@__PURE__*/ new EmojiGroup("\u{1F476}\uDC76", "Baby", ...allBabies);

export const allPeople = /*@__PURE__*/ [
    allBabiesGroup,
    allChildrenGroup,
    allPersonsGroup,
    allOlderPersonsGroup
];

export const allPeopleGroup = /*@__PURE__*/ new EmojiGroup("\u0050\u0065\u006F\u0070\u006C\u0065", "People", ...allPeople);

export const allCharacters = /*@__PURE__*/ [
    allPeopleGroup,
    allGesturingGroup,
    allInMotionGroup,
    allRestingGroup,
    allOccupationGroup,
    allFantasyGroup
];

export const allCharactersGroup = /*@__PURE__*/ new EmojiGroup("\u0041\u006C\u006C\u0020\u0050\u0065\u006F\u0070\u006C\u0065", "All People", ...allCharacters);

