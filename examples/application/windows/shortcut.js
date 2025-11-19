import {PathUtil, Command, Key, Color, Device} from "../../device/aergia_types.js";

let ShortcutActions = {
    location: PathUtil.getSourcePath(),
    Next: {
        visual: {
            background: Color.DarkBlue,
            fontSize: 1,
            text: "next"
        },
        command: Command.KeyInput,
        value: [Key.MediaNext]
    },
    Prev: {
        visual: {
            background: Color.DarkBlue,
            fontSize: 1,
            text: "prev"
        },
        command: Command.KeyInput,
        value: [Key.MediaPrevious]
    },
    Pause: {
        visual: {
            background: Color.DarkBlue,
            fontSize: 1,
            text: "pause"
        },
        command: Command.KeyInput,
        value: [Key.MediaPause]
    },
    Scroll: {
        visual: {
            background: Color.DarkBlue,
            fontSize: 1,
            text: "scroll"
        },
        event_wheel: {
            command: Command.MouseWheel,
            delta: Device.Wheel.Delta,
            r: -1
        }
    }
}

export const WindowsShortcut = {
    location: PathUtil.getSourcePath(),
    visual: {
        text: "Windows",
        color: Color.RGB(10, 10, 120),
        icon: "icon/windows.png",
        background: Color.RGB(80,80,80)
    },
    actions: ShortcutActions
}
