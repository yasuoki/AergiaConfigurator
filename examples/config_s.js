import {Command, Key, PathUtil} from "./device/aergia_types.js";
import {Matrix_s} from "./device/matrix_s.js";
import {DSMApplication} from './application/dsm/designspark_mechanical.js';
import {VMWApplication} from "./application/vmw/video_mastering_works7.js";
import {CalcApplication} from "./application/calc/calc.js";
import {ArkApplication} from "./application/ark/ark.js";
import {PlayerApplication} from "./application/windows/player.js";

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
ArkApplication.actions.showAttackPage = {
    visual: {
        background: 0x030505,
        text: "ATTACK",
        fontSize: 1,
    },
    event_keyDown: {
        command: Command.PageChange,
        page: "attackPage"
    },
}
ArkApplication.actions.showMovePage = {
    visual: {
        background: 0x030505,
        text: "MOVE",
        fontSize: 1,
    },
    event_keyUp: {
        commands: [
            {command: Command.KeyRelease, interval:5, value: [
                    Key.Key0,Key.Key1,Key.Key2,Key.Key3,Key.Key4,
                    Key.Key5,Key.Key6,Key.Key7,Key.Key8,Key.Key9]},
            {command: Command.PageChange, page: "movePage"}
        ]
    }
}
ArkApplication.actions.escape = {
    visual: {
        background: 0x030505,
        text: "ESC",
        fontSize: 2,
    },
    event_keyDown: {
        command: Command.KeyPress,
        value: [Key.Esc]
    },
    event_keyUp: {
        command: Command.KeyRelease,
        value: [Key.Esc]
    }
}

export const ConfigData = {
    location: PathUtil.getSourcePath(),
    device: Matrix_s,
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
        },
        ARK: {
            application: ArkApplication,
            binds: {
                movePage : {
                    KeySwitch12: "Run",         KeySwitch13: "MoveForward", KeySwitch14: "Jump",        KeySwitch15: "AllTracking",
                    KeySwitch8: "MoveLeft",     KeySwitch9: "Dash",         KeySwitch10: "MoveRight",   KeySwitch11: "AllStop",
                    KeySwitch4: "Prone",        KeySwitch5: "MoveBackward", KeySwitch6:  "Crouch",     KeySwitch7: "Attack",
                    KeySwitch0: "InventoryOpen",KeySwitch1: "ItemUse",       KeySwitch2:  "ToggleFists", KeySwitch3: "WhistleMenu",
                    Button0: "showAttackPage",
                    Button1: "escape",
                },
                attackPage : {
                    KeySwitch12: "UseSlot1",      KeySwitch13: "UseSlot2",  KeySwitch14: "UseSlot3",    KeySwitch15: "OneTracking",
                    KeySwitch8:  "UseSlot4",      KeySwitch9:  "UseSlot5",  KeySwitch10: "UseSlot6",    KeySwitch11: "OneStop",
                    KeySwitch4:  "UseSlot7",      KeySwitch5:  "UseSlot8",  KeySwitch6:  "UseSlot9",    KeySwitch7: "UseSlot0",
                    KeySwitch0:  "GhostMode",     KeySwitch1:  "WalkMode",    KeySwitch2:  "ToggleFists", KeySwitch3: "WhistleMenu",
                    Button0: "showMovePage",
                    Button1: "escape",
                }
            }
        },
        Player:{
            application: PlayerApplication,
            binds: {
                p:{
                    KeySwitch0: "Prev",
                    KeySwitch1: "Pause",
                    KeySwitch2: "Next"
                }
            }
        }
    }
}
