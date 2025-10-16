import {PathUtil, Command, Key, Color} from "../../device/aergia_types.js";

let PlayerActions = {
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
    }
}

export const PlayerApplication = {
    location: PathUtil.getSourcePath(),
    visual: {
        text: "player",
        color: Color.RGB(100, 100, 100),
        background: Color.RGB(0,0,60)
    },
    actions: PlayerActions
}
