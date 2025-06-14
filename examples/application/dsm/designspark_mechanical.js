import {PathUtil, Command, Device, Button, Key, Color} from "../../device/aergia_types.js";
// http://help.spaceclaim.com/dsm/6.0/en/index.html

const viewCommandColor = Color.RGB(10,10,10);
const keyCommandColor = Color.RGB(0,0,20);
const modeCommandColor = Color.Lime;
const editCommandColor = Color.DarkGreen;
const moveCommandColor = Color.Navy;
const copyCommandColor = Color.DarkViolet;
const sketchCommandColor = Color.DarkOrange;
const menuCommandColor = Color.SteelBlue;
const styleCommandColor = Color.DarkTurquoise;
const treeCommandColor = Color.Silver;
const windowCommandColor = Color.RoyalBlue;

let DSMActions = {
    location: PathUtil.getSourcePath(),
    AppOpen: {
        command: Command.KeyInput,
        value: [Key.LeftMeta, "s", "DesignSpark Mechanical 6.0", Key.Enter]
    },
    ViewSpin: {
        event_beginRotate: {
            commands: [
                {
                    command: Command.MouseTrackingStart
                },{
                    command: Command.ButtonPress,
                    button: Button.Middle
                }
            ]
        },
        event_rotate: {
            command:Command.MouseMove,
            x: Device.Joystick.X,
            y: Device.Joystick.Z,
        },
        event_endRotate: {
            commands:[
                {
                    command: Command.ButtonRelease,
                    button: Button.Middle
                },{
                    command: Command.MouseTrackingRewind
                }
            ]
        }
    },
    ViewPan: {
        event_beginMove: {
            commands: [
                {
                    command: Command.MouseTrackingStart
                },{
                    command: Command.KeyPress,
                    value: [Key.Shift]
                },{
                    command: Command.ButtonPress,
                    button: Button.Middle
                }
            ]
        },
        event_move: {
            command:Command.MouseMove,
            x: Device.Joystick.X,
            y: Device.Joystick.Z,
        },
        event_endMove: {
            commands:[
                {
                    command: Command.KeyRelease,
                    value: [Key.Shift]
                },{
                    command: Command.ButtonRelease,
                    button: Button.Middle
                },{
                    command: Command.MouseTrackingRewind
                }
            ]
        }
    },
    ViewZoom: {
        command: Command.MouseWheel,
        delta: Device.Wheel.Delta,
    },
    ViewPlan: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_plan.png"
        },
        command: Command.KeyInput,
        value: ["v"]
    },
    ViewZoomExtent: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_zoom_extent.png"
        },
        command: Command.KeyInput,
        value: ["z"]
    },
    ViewZoomBoxIn: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_zoom_box_in.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt,"z"]
    },
    ViewSnapView: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_snap_view.png"
        },
        commands: [
            {
                command: Command.KeyPress,
                value: [Key.Shift]
            },{
                command: Command.DoubleClick,
                button: Button.Middle
            },{
                command: Command.KeyRelease,
                value: [Key.Shift]
            }
        ]
    },
    ViewPreviousView: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_previous_view.png"
        },
        command: Command.KeyInput,
        value: [Key.LeftArrow]
    },
    ViewNextView: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_next_view.png"
        },
        command: Command.KeyInput,
        value: [Key.RightArrow]
    },
    ViewHome: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_home.png"
        },
        command: Command.KeyInput,
        value: ["h"]
    },
    ViewHideObject: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_hide_object.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "h"]
    },
    ViewHideOthers: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_hide_others.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt, "h"]
    },
    ViewZoomIn: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_zoom_in.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,Key.NumpadPlus]
    },
    ViewZoomOut: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_zoom_out.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,Key.NumpadMinus]
    },
    ViewSetSpinCenter: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_set_spin_center.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"t"]
    },
    ViewLocateSpinCenter: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_locate_spin_center.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"l"]
    },
    ViewClearSpinCenter: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_clear_spin_center.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"q"]
    },
    ViewShowAll: {
        visual: {
            background: viewCommandColor,
            icon: "icon/view_show_all.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.Shift, "h"]
    },
    EnterKey: {
        visual: {
            background: keyCommandColor,
            icon: "icon/key_enter.png"
        },
        command: Command.KeyInput,
        value: [Key.Enter]
    },
    ESCKey: {
        visual: {
            background: keyCommandColor,
            icon: "icon/key_escape.png"
        },
        command: Command.KeyInput,
        value: [Key.Esc]
    },
    Mode3D: {
        visual: {
            background: modeCommandColor,
            icon: "icon/mode_3d.png"
        },
        command: Command.KeyInput,
        value: ["d"]
    },
    ModeSection: {
        visual: {
            background: modeCommandColor,
            icon: "icon/mode_section.png"
        },
        command: Command.KeyInput,
        value: ["x"]
    },
    ModeSketch: {
        visual: {
            background: modeCommandColor,
            icon: "icon/mode_sketch.png"
        },
        command: Command.KeyInput,
        value: ["k"]
    },
    EditSelect: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_select.png"
        },
        command: Command.KeyInput,
        value: ["s"]
    },
    EditSelectAllSameType: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_select_all_same_type.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "a"]
    },
    EditInvertSelection: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_invert_selection.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.Shift, "i"]
    },
    EditBlend: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_blend.png"
        },
        command: Command.KeyInput,
        value: ["b"]
    },
    EditCombine: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_combine.png"
        },
        command: Command.KeyInput,
        value: ["i"]
    },
    EditFill: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_fill.png"
        },
        command: Command.KeyInput,
        value: ["f"]
    },
    EditPull: {
        visual: {
            background: editCommandColor,
            icon: "icon/edit_pull.png"
        },
        command: Command.KeyInput,
        value: ["p"]
    },
    EditMove: {
        visual: {
            background: moveCommandColor,
            icon: "icon/edit_move.png"
        },
        command: Command.KeyInput,
        value: ["m"]
    },
    EditMoveSketchGridIn: {
        visual: {
            background: moveCommandColor,
            icon: "icon/edit_move_sketch_grid_in.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.RightArrow]
    },
    EditMoveSketchGridOut: {
        visual: {
            background: moveCommandColor,
            icon: "icon/edit_move_sketch_grid_out.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.LeftArrow]
    },
    EditCopy: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_copy.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"c"]
    },
    EditCopyFaces: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_copy_faces.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,Key.Shift,"c"]
    },
    EditCut: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_cut.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"x"]
    },
    EditCutFaces: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_cut_faces.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,Key.Shift,"x"]
    },
    EditPaste: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_paste.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "v"]
    },
    EditDelete: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_delete.png"
        },
        command: Command.KeyInput,
        value: [Key.Delete]
    },
    EditTrimAway: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_trim_away.png"
        },
        command: Command.KeyInput,
        value: ["t"]
    },
    EditUndo: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_undo.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"z"]
    },
    EditRedo: {
        visual: {
            background: copyCommandColor,
            icon: "icon/edit_redo.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"y"]
    },
    SketchCircle: {
        visual: {
            background: sketchCommandColor,
            icon: "icon/sketch_circle.png"
        },
        command: Command.KeyInput,
        value: ["c"]
    },
    SketchLine: {
        visual: {
            background: sketchCommandColor,
            icon: "icon/sketch_line.png"
        },
        command: Command.KeyInput,
        value: ["l"]
    },
    SketchRectangle: {
        visual: {
            background: sketchCommandColor,
            icon: "icon/sketch_rectangle.png"
        },
        command: Command.KeyInput,
        value: ["r"]
    },

    RadialMenu: {
        visual: {
            background: menuCommandColor,
            icon: "icon/radial_menu.png"
        },
        command: Command.KeyInput,
        value: [Key.O]
    },
    InspectMeasure: {
        visual: {
            background: modeCommandColor,
            icon: "icon/inspect_measure.png"
        },
        command: Command.KeyInput,
        value: ["e"]
    },
    UpToToolGuide: {
        visual: {
            background: editCommandColor,
            icon: "icon/tool_guide.png"
        },
        command: Command.KeyInput,
        value: ["u"]
    },
    WindowNextDesign: {
        visual: {
            background: windowCommandColor,
            icon: "icon/window_next_design.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.Tab]
    },
    WindowPreviousDesign: {
        visual: {
            background: windowCommandColor,
            icon: "icon/window_next_design.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.Shift, Key.Tab]
    },
    FileExit: {
        visual: {
            background: windowCommandColor,
            icon: "icon/file_exit.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt, Key.F4]
    },
    MenuFile: {
        visual: {
            background: menuCommandColor,
            icon: "icon/menu_file.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt,"f"]
    },
    FileClose: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_close.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.F4]
    },
    FileNew: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_new.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "n"]
    },
    FileOpen: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_open.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "o"]
    },
    FilePrint: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_print.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "p"]
    },
    FilePrintPreview: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_print_preview.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.F2]
    },
    FileSave: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_save.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,"s"]
    },
    FileSaveAs: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_save_as.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl,Key.Shift, "s"]
    },
    FileSaveAsNew: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_save_as_new.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt,Key.Shift, "s"]
    },
    FileShareAsFile: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_share_as_file.png"
        },
        command: Command.KeyInput,
        value: [Key.Alt, "s"]
    },
    FileShareAsNewVersion: {
        visual: {
            background: menuCommandColor,
            icon: "icon/file_share_as_new_version.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.Alt, "s"]
    },
    StyleBoldText: {
        visual: {
            background: styleCommandColor,
            icon: "icon/style_bold_text.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "b"]
    },
    StyleItalicText: {
        visual: {
            background: styleCommandColor,
            icon: "icon/style_italic_text.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "i"]
    },
    StyleUnderlineText: {
        visual: {
            background: styleCommandColor,
            icon: "icon/style_underline_text.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, "u"]
    },
    ActivateComponent: {
        visual: {
            background: treeCommandColor,
            icon: "icon/activate_component.png"
        },
        command: Command.KeyInput,
        value: [Key.Ctrl, Key.Shift, "a"]
    },
    ExpandEntireTree: {
        visual: {
            background: treeCommandColor,
            icon: "icon/expand_entire_tree.png"
        },
        command: Command.KeyInput,
        value: [Key.NumpadMul]
    },
    ExpandSelectedTree: {
        visual: {
            background: treeCommandColor,
            icon: "icon/expand_selected_tree.png"
        },
        command: Command.KeyInput,
        value: [Key.NumpadPlus]
    },
    CollapseSelectedTree: {
        visual: {
            background: treeCommandColor,
            icon: "icon/collapse_selected_tree.png"
        },
        command: Command.KeyInput,
        value: [Key.NumpadMinus]
    }
}

export let DSMApplication = {
    location:PathUtil.getSourcePath(),
    visual: {
        text: "DSM",
        color: Color.Tomato,
        icon: "icon/dsm.png",
        background: Color.RGB(0x20,0x20,0x20)
    },
    actions: DSMActions
}
