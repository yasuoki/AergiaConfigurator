//
// ARK: Survival Ascended Application
import {PathUtil, Command, Key, Color} from "../../device/aergia_types.js";

const moveCommandColor = 0x000030;
const inventoryColor = 0x002000;
const petCommandColor = 0x301000;
const consoleCommandColor = 0x300030;
const consoleKey = Key.JP_YEN;

let ARKActions = {
    location: PathUtil.getSourcePath(),
    MoveForward: {
        visual: {
            background: moveCommandColor,
            icon: "icon/move_forward.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.W]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.W]
        }
    },
    MoveBackward: {
        visual: {
            background: moveCommandColor,
            icon: "icon/move_backward.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.S]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.S]
        }
    },
    MoveLeft: {
        visual: {
            background: moveCommandColor,
            icon: "icon/move_left.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.A]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.A]
        }
    },
    MoveRight: {
        visual: {
            background: moveCommandColor,
            icon: "icon/move_right.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.D]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.D]
        }
    },
    Run: {
        visual: {
            background: moveCommandColor,
            icon: "icon/run.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.LeftShift]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.LeftShift]
        }
    },
    Dash: {
        visual: {
            background: moveCommandColor,
            icon: "icon/dash.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.RightShift]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.RightShift]
        }
    },
    Jump: {
        visual: {
            background: moveCommandColor,
            icon: "icon/jump.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Space]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Space]
        }
    },
    Prone: {
        visual: {
            background: moveCommandColor,
            icon: "icon/prone.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.X]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.X]
        }
    },
    Crouch: {
        visual: {
            background: moveCommandColor,
            icon: "icon/crouch.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.C]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.C]
        }
    },
    InventoryOpen: {
        visual: {
            background: inventoryColor,
            icon: "icon/inventory.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.I]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.I]
        }
    },
    Inventory2Open: {
        visual: {
            background: inventoryColor,
            icon: "icon/inventory2.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.F]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.F]
        }
    },
    ItemUse: {
        visual: {
            background: inventoryColor,
            icon: "icon/item_use.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.E]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.E]
        }
    },
    UseSlot0: {
        visual: {
            background: inventoryColor,
            text: "10",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["0"]
    },
    UseSlot1: {
        visual: {
            background: inventoryColor,
            text: "1",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["1"]
    },
    UseSlot2: {
        visual: {
            background: inventoryColor,
            text: "2",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["2"]
    },
    UseSlot3: {
        visual: {
            background: inventoryColor,
            text: "3",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["3"]
    },
    UseSlot4: {
        visual: {
            background: inventoryColor,
            text: "4",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["4"]
    },
    UseSlot5: {
        visual: {
            background: inventoryColor,
            text: "5",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["5"]
    },
    UseSlot6: {
        visual: {
            background: inventoryColor,
            text: "6",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["6"]
    },
    UseSlot7: {
        visual: {
            background: inventoryColor,
            text: "7",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["7"]
    },
    UseSlot8: {
        visual: {
            background: inventoryColor,
            text: "8",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["8"]
    },
    UseSlot9: {
        visual: {
            background: inventoryColor,
            text: "9",
            fontSize:2,
        },
        command: Command.KeyInput,
        value: ["9"]
    },
    ToggleFists: {
        visual: {
            background: inventoryColor,
            icon: "icon/toggle_fists.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Q]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Q]
        }
    },
    Reload: {
        visual: {
            background: inventoryColor,
            icon: "icon/reload.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.R]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.R]
        }
    },
    MapOpen: {
        visual: {
            background: inventoryColor,
            icon: "icon/map.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.M]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.M]
        }
    },
    MapMark: {
        visual: {
            background: inventoryColor,
            icon: "icon/map_mark.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.P]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.P]
        }
    },
    WhistleMenu: {
        visual: {
            background: inventoryColor,
            icon: "icon/whistleMenu.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Delete]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Delete]
        }
    },

    AllTracking: {
        visual: {
            background: petCommandColor,
            icon: "icon/all_tracking.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.J]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.J]
        }
    },
    OneTracking: {
        visual: {
            background: petCommandColor,
            icon: "icon/one_tracking.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.T]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.T]
        }
    },
    AllStop: {
        visual: {
            background: petCommandColor,
            icon: "icon/stop_all.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.U]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.U]
        }
    },
    OneStop: {
        visual: {
            background: petCommandColor,
            icon: "icon/stop_one.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Y]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Y]
        }
    },
    Attack: {
        visual: {
            background: petCommandColor,
            icon: "icon/attack.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.RightCtrl]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.RightCtrl]
        }
    },
    AttackTo: {
        visual: {
            background: petCommandColor,
            icon: "icon/attack_to.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Dot]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Dot]
        }
    },
    Impartial: {
        visual: {
            background: petCommandColor,
            icon: "icon/impartial.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.End]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.End]
        }
    },
    NoResistance: {
        visual: {
            background: petCommandColor,
            icon: "icon/no_resistance.png"
        },
        event_keyDown: {
            command: Command.KeyPress,
            value: [Key.Semicolon]
        },
        event_keyUp: {
            command: Command.KeyRelease,
            value: [Key.Semicolon]
        }
    },
    GhostMode: {
        visual: {
            background: consoleCommandColor,
            icon: "icon/ghost.png"
        },
        command: Command.KeyInput,
        value: [consoleKey, "ghost", Key.Enter]
    },
    WalkMode: {
        visual: {
            background: consoleCommandColor,
            icon: "icon/walk.png"
        },
        command: Command.KeyInput,
        value: [consoleKey, "walk", Key.Enter]
    }
}

export let ArkApplication = {
    location:PathUtil.getSourcePath(),
    visual: {
        text: "ARK",
        color: Color.SteelBlue,
        icon: "icon/logo.png",
        background: Color.Black
    },
    actions: ARKActions
}
