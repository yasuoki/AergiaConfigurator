import {PathUtil, Command, Device, Button, Key, Color} from "../../device/aergia_types.js";
// https://help.pegasys-inc.com/ja/tvmw7/00130.html

const playControlColor = Color.RGB(10,10,10);
const move1Color = Color.RGB(1,0,1);
const move5Color = Color.RGB(5,0,5);
const move10Color = Color.RGB(10,0,10);
const moveSColor = Color.RGB(15,0,15);
const markColor = Color.RGB(0,0,15);
const moveMarkColor = Color.RGB(10,0,15);

const VMWActions = {
    location: PathUtil.getSourcePath(),
    AppOpen: {
        command: Command.KeyInput,
        value: [Key.LeftMeta, "s", "TMPGEnc Video Mastering Works 7", Key.Enter]
    },
    PlayPause: {
        visual: {
            background: playControlColor,
            icon: "icon/play_pause.png"
        },
        command: Command.KeyInput,
        value: [Key.Space]
    },
    Stop: {
        visual: {
            background: playControlColor,
            icon: "icon/stop.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt, Key.Ctrl, Key.Shift, "s"]
    },
    FastPlay: {
        visual: {
            background: playControlColor,
            icon: "icon/fast_play.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Ctrl, Key.Shift, "p"]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: ["p", Key.Shift, Key.Ctrl]
        }
    },
    FastRewind: {
        visual: {
            background: playControlColor,
            icon: "icon/fast_rewind.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Ctrl, Key.Shift, "b"]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: ["b", Key.Shift, Key.Ctrl]
        }
    },
    NextFrame: {
        visual: {
            background: move1Color,
            icon: "icon/next_frame.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Ctrl, Key.Shift, Key.RightArrow]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.RightArrow, Key.Shift, Key.Ctrl]
        }
    },
    PreviousFrame: {
        visual: {
            background: move1Color,
            icon: "icon/previous_frame.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Ctrl, Key.Shift, Key.LeftArrow]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.LeftArrow, Key.Shift, Key.Ctrl]
        }
    },
    Next5Frame: {
        visual: {
            background: move1Color,
            icon: "icon/next_5frame.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.PageDown]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.PageDown]
        }
    },
    Previous5Frame: {
        visual: {
            background: move5Color,
            icon: "icon/previous_5frame.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.PageUp]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.PageUp]
        }
    },
    Next10Frame: {
        visual: {
            background: move10Color,
            icon: "icon/next_10frame.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.PageDown]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.PageDown]
        }
    },
    Previous10Frame: {
        visual: {
            background: move10Color,
            icon: "icon/previous_10frame.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.PageUp]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.PageUp]
        }
    },
    NextScene: {
        visual: {
            background: moveSColor,
            icon: "icon/next_scene.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.DownArrow]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.DownArrow]
        }
    },
    PreviousScene: {
        visual: {
            background: moveSColor,
            icon: "icon/previous_scene.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.UpArrow]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.UpArrow]
        }
    },
    MarkInPoint: {
        visual: {
            background: markColor,
            icon: "icon/mark_in_point.png"
        },
        command: Command.KeyInput,
        value: ["["]
    },
    MarkOutPoint: {
        visual: {
            background: markColor,
            icon: "icon/mark_out_point.png"
        },
        command: Command.KeyInput,
        value: ["]"]
    },
    ResetInOutPoint: {
        visual: {
            background: markColor,
            icon: "icon/reset_in_out_point.png"
        },
        command: Command.KeyInput,
        value: [Key.Shift,"a"]
    },
    MoveInPoint: {
        visual: {
            background: moveMarkColor,
            icon: "icon/move_in_point.png"
        },
        command: Command.KeyInput,
        value: [Key.Shift,Key.Home]
    },
    MoveOutPoint: {
        visual: {
            background: moveMarkColor,
            icon: "icon/move_out_point.png"
        },
        command: Command.KeyInput,
        value: [Key.Shift,Key.End]
    },
    DeleteInOut: {
        visual: {
            background: markColor,
            icon: "icon/delete_in_out.png"
        },
        command: Command.KeyInput,
        value: [Key.Shift,Key.Delete]
    },
    SpeedSeek: {
        command: Command.MapInput,
        interval: 60,
        key: Device.Joystick.Z,
        map: [
            {le: -11, value: [Key.Ctrl, Key.Shift, Key.Key9]},
            {le: -10, value: [Key.Ctrl, Key.Shift, Key.Key8]},
            {le: -9, value: [Key.Ctrl, Key.Shift, Key.Key7]},
            {le: -8, value: [Key.Ctrl, Key.Shift, Key.Key6]},
            {le: -7, value: [Key.Ctrl, Key.Shift, Key.Key5]},
            {le: -6, value: [Key.Ctrl, Key.Shift, Key.Key4]},
            {le: -5, value: [Key.Ctrl, Key.Shift, Key.Key3]},
            {le: -4, value: [Key.Ctrl, Key.Shift, Key.Key2]},
            {le: -3, value: [Key.Ctrl, Key.Shift, Key.Key1]},
            {le: 4, value: [Key.Ctrl, Key.Key1]},
            {le: 5, value: [Key.Ctrl, Key.Key2]},
            {le: 6, value: [Key.Ctrl, Key.Key3]},
            {le: 7, value: [Key.Ctrl, Key.Key4]},
            {le: 8, value: [Key.Ctrl, Key.Key5]},
            {le: 9, value: [Key.Ctrl, Key.Key6]},
            {le: 10, value: [Key.Ctrl, Key.Key7]},
            {le: 11, value: [Key.Ctrl, Key.Key8]},
            {le: 99, value: [Key.Ctrl, Key.Key9]}
        ]
    },
    WheelMove: {
        event_wheel: {
            command: Command.MouseWheel,
            delta: Device.Wheel.Delta,
            r: -1
        }
    },
    // basic key
    EnterKey: {
        visual: {
            background: 0x000400,
            icon: "icon/key_enter.png",
        },
        command: Command.KeyInput,
        value: [Key.Enter]
    },
    ESCKey: {
        visual: {
            background: 0x000400,
            icon: "icon/key_escape.png",
        },
        command: Command.KeyInput,
        value: [Key.Esc]
    },
    AltKey: {
        visual: {
            background: 0x000400,
            icon: "icon/key_escape.png",
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Alt]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Alt]
        }
    }
}

export const VMWApplication = {
    location: PathUtil.getSourcePath(),
    visual: {
        text: "VMW",
        color: Color.RGB(100, 100, 0),
        icon: "icon/wmw96x96.png",
        background: Color.RGB(20,0,40)
    },
    actions: VMWActions
}

