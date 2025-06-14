import {PathUtil, Command, Key, Color} from "../../device/aergia_types.js";
// https://help.pegasys-inc.com/ja/tvmw7/00130.html

const numberColor = Color.RGB(0,20,0);
const opColor = Color.RGB(0,0,20);
const enterColor = Color.RGB(10,10,10);
const clearColor = Color.RGB(10,0,0);

const CalcActions = {
    location: PathUtil.getSourcePath(),
    // App change
    AppOpen: {
        visual: {
            color: Color.RGB(0, 0, 80),
            icon: "icon/calc_small.png"
        },
        command: Command.KeyInput,
        value: [Key.LeftMeta, "r", "calc", Key.Enter]
    },
    Key0: {
        visual: {
            background: numberColor,
            fontSize:2,
            text: "0"
        },
        command: Command.KeyInput,
        value: ["0"]
    },
    Key00: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "00"
        },
        command: Command.KeyInput,
        value: ["00"]
    },
    Key1: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "1"
        },
        command: Command.KeyInput,
        value: ["1"]
    },
    Key2: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "2"
        },
        command: Command.KeyInput,
        value: ["2"]
    },
    Key3: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "3"
        },
        command: Command.KeyInput,
        value: ["3"]
    },
    Key4: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "4"
        },
        command: Command.KeyInput,
        value: ["4"]
    },
    Key5: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "5"
        },
        command: Command.KeyInput,
        value: ["5"]
    },
    Key6: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "6"
        },
        command: Command.KeyInput,
        value: ["6"]
    },
    Key7: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "7"
        },
        command: Command.KeyInput,
        value: ["7"]
    },
    Key8: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "8"
        },
        command: Command.KeyInput,
        value: ["8"]
    },
    Key9: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "9"
        },
        command: Command.KeyInput,
        value: ["9"]
    },
    KeyDot: {
        visual: {
            background: numberColor,
            fontSize: 2,
            text: "."
        },
        command: Command.KeyInput,
        value: ["."]
    },
    KeyPlus: {
        visual: {
            background: opColor,
            icon: "icon/plus.png",
            fontSize: 2,
            text: "+"
        },
        command: Command.KeyInput,
        value: [Key.NumpadPlus]
    },
    KeyMinus: {
        visual: {
            background: opColor,
            icon: "icon/minus.png",
            fontSize: 2,
            text: "-"
        },
        command: Command.KeyInput,
        value: [Key.NumpadMinus]
    },
    KeyMul: {
        visual: {
            background: opColor,
            icon: "icon/mul.png",
            fontSize: 2,
            text: "*"
        },
        command: Command.KeyInput,
        value: [Key.NumpadMul]
    },
    KeyDiv: {
        visual: {
            background: opColor,
            icon: "icon/div.png",
            fontSize: 2,
            text: "/"
        },
        command: Command.KeyInput,
        value: ["/"]
    },
    KeyEq: {
        visual: {
            background: enterColor,
            icon: "icon/eq.png",
            fontSize: 2,
            text: "="
        },
        command: Command.KeyInput,
        value: [Key.Enter]
    },
    KeyClear: {
        visual: {
            background: clearColor,
            fontSize: 2,
            text: "C"
        },
        command: Command.KeyInput,
        value: [Key.Esc]
    },
}

export const CalcApplication = {
    location: PathUtil.getSourcePath(),
    visual: {
        text: "Calculator",
        color: Color.RGB(100, 100, 100),
        icon: "icon/calc.png",
        background: Color.RGB(0,0,60)
    },
    actions: CalcActions
}

