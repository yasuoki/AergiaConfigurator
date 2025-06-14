import {Command, PathUtil} from "./device/aergia_types.js";
import {Matrix} from "./device/matrix.js";
import {DSMApplication} from './application/dsm/designspark_mechanical.js';
import {VMWApplication} from "./application/vmw/video_mastering_works7.js";
import {CalcApplication} from "./application/calc/calc.js";

DSMApplication.actions.show3dPage = {
    command: Command.PageChange,
    page: "page3d"
}
DSMApplication.actions.showSketchPage = {
    command: Command.PageChange,
    page: "pageSketch"
}
DSMApplication.actions.showSectionPage = {
    command: Command.PageChange,
    page: "pageSection"
}

export const ConfigData = {
    location: PathUtil.getSourcePath(),
    device: Matrix,
    applications: {
        DSM: {
            application: DSMApplication,
            binds: {
                pageSketch: {
                    Wheel:"ViewZoom",
                    KeySwitch0: "ViewHome",
                    KeySwitch1: "ViewZoomExtent",
                    KeySwitch2: "ViewPlan",
                    KeySwitch3: "EnterKey",

                    KeySwitch4: "EditFill",
                    KeySwitch5: "EditFill",
                    KeySwitch6: "EditMove",
                    KeySwitch7: "EditSelect",

                    KeySwitch12: "ESCKey",
                    KeySwitch13: "ViewHome",
                    KeySwitch14: ["ModeSection","showSketchPage"],
                    KeySwitch15: ["Mode3D", "show3dPage"]
                },
                page3d: {
                    Wheel:"ViewZoom",
                    KeySwitch0: "ESCKey",
                    KeySwitch1: "EnterKey",
                    KeySwitch2: "ViewPlan",
                    KeySwitch3: "ViewHome",
                    KeySwitch4: "EditCombine",
                    KeySwitch5: "EditPull",
                    KeySwitch6: "EditMove",
                    KeySwitch7: "EditSelect",
                    KeySwitch14: ["ModeSection","showSketchPage"],
                    KeySwitch15: ["ModeSection", "showSectionPage"]
                },
                pageSection: {
                    Wheel:"ViewZoom",
                    KeySwitch0: "ESCKey",
                    KeySwitch1: "EnterKey",
                    KeySwitch2: "ViewPlan",
                    KeySwitch3: "ViewHome",
                    KeySwitch4: "EditFill",
                    KeySwitch5: "EditFill",
                    KeySwitch6: "EditMove",
                    KeySwitch7: "EditSelect",
                    KeySwitch14: ["Mode3D","show3dPage"],
                    KeySwitch15: ["ModeSection", "showSectionPage"]
                }
            }
        },
        VWM: {
            application: VMWApplication,
            binds: {
                p: {
                    KeySwitch0:  "FastRewind",
                    KeySwitch1:  "PlayPause",
                    KeySwitch2:  "Stop",
                    KeySwitch3:  "FastPlay",

                    KeySwitch4:  "MoveInPoint",
                    KeySwitch5:  "MarkInPoint",
                    KeySwitch6:  "MarkOutPoint",
                    KeySwitch7:  "MoveOutPoint",

                    KeySwitch8: "Previous5Frame",
                    KeySwitch9: "PreviousFrame",
                    KeySwitch10: "NextFrame",
                    KeySwitch11: "Next5Frame",

                    KeySwitch12: "PreviousScene",
                    KeySwitch13: "Previous10Frame",
                    KeySwitch14: "Next10Frame",
                    KeySwitch15: "NextScene",

                    Button0: "AltKey",
                    Button1: "ESCKey"
                }
            }
        },
        Calc: {
            application: CalcApplication,
            binds: {
                p : {
                    KeySwitch0: "Key0",
                    KeySwitch1: "Key00",
                    KeySwitch2: "KeyDot",
                    KeySwitch3: "KeyMinus",
                    KeySwitch4: "Key1",
                    KeySwitch5: "Key2",
                    KeySwitch6: "Key3",
                    KeySwitch7: "KeyPlus",
                    KeySwitch8: "Key4",
                    KeySwitch9: "Key5",
                    KeySwitch10: "Key6",
                    KeySwitch11: "KeyMul",
                    KeySwitch12: "Key7",
                    KeySwitch13: "Key8",
                    KeySwitch14: "Key9",
                    KeySwitch15: "KeyDiv",
                    Button0: "KeyEq",
                    Button1: "KeyClear",
                }
            }
        }
    }
}
