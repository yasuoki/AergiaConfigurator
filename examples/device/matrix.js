import {PathUtil, PixelFormat} from "./aergia_types.js"

const DisplayKeySwitchControl = {
    visualCapabilities: {
        text: {
            pixelFormat: PixelFormat.BW1
        },
        icon: {
            pixelFormat: PixelFormat.BW1,
            resolution: {
                h: 64,
                v: 32
            }
        },
        background: {
            pixelFormat: PixelFormat.Rgb888
        }
    },
    events: [
        "KeyDown",
        "KeyUp",
        "KeyInput",
        "LongPress",
        "Timer"
    ],
    variables: [
        "Status",
        "TimerData"
    ]
}

const ButtonControl = {
    visualCapabilities: {
        background: {
            pixelFormat: PixelFormat.Rgb888
        }
    },
    events: [
        "KeyDown",
        "KeyUp",
        "KeyInput",
        "LongPress",
        "Timer"
    ],
    variables: [
        "Status",
        "TimerData"
    ]
}

let controls = {}
controls.Display = {
    visualCapabilities: {
        text: {
            pixelFormat: PixelFormat.Rgb565
        },
        icon: {
            pixelFormat: PixelFormat.Rgb565,
            resolution: {
                h: 96,
                v: 96
            }
        },
        background: {
            pixelFormat: PixelFormat.Rgb565
        }
    },
    events: [
        "Timer"
    ],
    variables: [
        "TimerData"
    ]
}

controls.Wheel = {
    events: [
        "BeginWheel",
        "Wheel",
        "EndWheel",
        "Timer"
    ],
    variables: [
        "Delta",
        "TimerData"
    ]
}

controls.Ranging = {
    events: [
        "Enter",
        "Leave",
        "Timer"
    ],
    variables: [
        "Status",
        "Distance",
        "TimerData"
    ]
}

controls.Main = {
    events: [
        "Load",
        "Connect",
        "Disconnect",
        "Timer"
    ],
    variables: [
        "TimerData"
    ]
}

for (let i = 0; i < 16; i++) {
    controls["KeySwitch" + i] = DisplayKeySwitchControl;
}
controls.Button0 = ButtonControl;
controls.Button1 = ButtonControl;

// device capabilities
export const Matrix = {
    location: PathUtil.getSourcePath(),
    model: "Aergia Matrix",
    lang: "jp",
    controls: controls
}

