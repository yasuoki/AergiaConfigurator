# Aergia Configurator
## Aergia Devices Configration File
コンフィグレーションファイルは、JavaScriptで記述し、次のような構造を持つConfigDataオブジェクトをエクスポートします。

```javascript
import {Matrix} from './device/matrix.js';
export const ConfigData = {
    device: Matrix,
    applications: {
        // App1 アプリケーションに関する定義
        App1 : {
            // App1アプリケーションの動作定義
            application: {
                // App1アプリケーションの表示設定（スティック上の円形ディスプレイの表示）
                visual: {
                    icon: "icon/app1.png",
                    text: "App1",
                    color: Color.Green,
                    background: Color.Black
                },
                // App1アプリケーションのアクション定義（ショートカットなどをここで定義します）
                actions: {
                    // ViewSectionアクションの定義
                    ViewSection: {
                        // ViewSectionアクションの表示定義（ボタンなどに表示されるアイコンや文字）
                        visual: {
                            icon: "icon/view_section.png",
                            background: Color.rgb(0,0,60),
                        },
                        command: Command.KeyInput,
                        value: ["x"]
                    },
                    // View３Dアクションの定義
                    View3D: {
                        :
                    },
                    :
                }
            },
            // バインド定義。デバイスのコントロールに実行するアクションを割り当てます
            binds: {
                // page3dのバインド定義
                page3d: {
                    keySwitch0: "ViewSketch",   // keySwitch0を押したときに実行するアクション
                    keySwitch1: "ViewSection",  // keySwitch1を押したときに実行するアクション
                        :
                },
                // pageCketchのバインド定義
                pageSketch: {
                    keySwitch0: "View3D",       // keySwitch0を押したときに実行するアクション
                    keySwitch1: "ViewSection",  // keySwitch1を押したときに実行するアクション
                      :
                },
                :
            }
        },
        // App2 アプリケーションに関する定義
        App2: {
            :
        }
    }
}
```
### device
設定対象のデバイスを示し、インポートしたFalconやMatrixなどを指定します。
### applications
アプリケーション別の設定を定義するオブジェクトです。  

## application
アプリケーションの動作を定義します。  
この定義は対象となるデバイスの具体的な機能に関連せず、すべてのAergiaデバイスで共通に使用できます。  
通常、次のようにアプリケーションの動作設定を別のファイルとして作成してエクスポートし、メインの設定ファイルからimportして使用します。

```javascript
import {PathUtil, Command, Device, Button, Key, Color} from "../../device/aergia_types.js";

export const App1 = {
    visual: {
        // 表示属性
    },
    commands: {
        // アクション定義
    }
}
```

### visual
デバイスに表示されるアプリケーション情報の表示形式を定義します。  
デバイスによって表示機能が異なりますが、対象デバイスにどのような表示ができるのかは、デバイス定義ファイル（device/matrix.js等）で確認してください。  

```javascript
export const App1 = {
    visual: {
        text: "application",
        icon: "application.png",
        color: Color.AliceBlue,
        background: Color.White
    }
    :
}

```
- text
アプリケーションを示す文字列を指定します。
- icon
アプリケーションを示すアイコンファイルを指定します。   
アイコンファイルは、jpegまたはpng形式の画像を指定することができ、対象デバイスの表示能力に応じてサイズは拡大・縮小され、色は減色されます。
拡大・縮小により画像が乱れるため、あらかじめ表示能力にマッチするサイズの画像を用意したほうが良い結果になります。
- color  
textを表示する際の色を指定します。  
- background  
背景の色を指定します。

## actions  
実行する動作群を定義します。  
actionsでアプリケーションで利用可能なキーボードショートカットやマウス操作などを定義し、bindsで実際に利用する機能をデバイスのボタンやスティックに割り当てます。

```javascript
export const App1 = {
    :
    actions: {
        ViewHome: {
            visual: {
                text: "HOME"
                color: Color.rgb(0,60,0),
                icon: "../icon/view_home.png",
                background: Color.DarkGreen
            },
            command: Command.KEY_INPUT,
            value: ["h"]
        },
        ViewPan: {
            :
        },
        :
    }
}
```
- object name(ViewHomeなど)  
動作の名前です。バインディングする際に参照するので、わかりやすい名前を付与してください。
- visual  
機能をバインドしたキーなどに表示する際の表示形式を定義します。定義内容はapplication.visualと同じです。  
visualの指定は任意で、指定しない場合はなにも表示されません。  
デバイスの種類やハードウェア構成により表示可能な形式が異なることに留意してください。
- command  
実行する操作を指定します。  
詳細は後述する「command」を参照してください。  
- value  
commandの種類ごとに定義されるパラメータです。  
  詳細は後述する「command」を参照してください。  

## binds
actionsで定義された動作を、デバイスのコントロール(操作部)に割り当てます。  
バインド情報は複数のページで定義し、アプリケーションの状態やモードに合わせてページを切り替えて利用します。  
各ページの定義内容は、基本的には`デバイスのコントロールの名前: "アクション名"`の形式で指定します。  
コントロールの名前はデバイスの種類により異なり、例えば、falconデバイスの場合には、joystick, button0, wheel, keySwitch0 ~ keySwitch7のコントロールがあり、matrixデバイスの場合には、wheel, button0, button1, keySwitch0 ~ keySwitch15, rangingのコントロールがあります。   
デバイスにどのようなコントロールがあるのか、デバイス定義ファイル（device/matrix.js等）で確認してください。

```javascript
export const ConfigData = {
    :
    applications: {
        App1: {
            application: {
                visual: {
                    :
                }
                ,
                commands: {
                    :
                }
            },
            binds : {
                page3d: {
                    keySwitch0: "ViewHome", 
                    keySwitch1: "ViewPlan",
                        :
                },
                pageSketch: {
                    :
                },
                :
            }
        },
        App2 : {
            :
        },
        :
    }
}
```

### page3d - ページ名  
ページの名前です。  
ページ切り替え時に参照するので、わかりやすい名前を付与してください。

### keySwitch0, keySwitch1
アクションを割り当てるキーやホイールなどのコントロールを指定します。  
もっともシンプルな定義は、コントロール名に対してアクション名を指定します。
```javascript
    keySwitch0: "ViewMode"
```
このように記述することで、keySwitch0に"ViewHome"アクションが割り当てられ、actions.ViewHome.visualで定義される形式でボタンが表示されます。  

さらに詳細な動作を定義するためには、コントロールの操作により発生する`イベント`に対して機能を割り当てます。
```javascript
    keySwitch0: {
        keyDown: "actionA",
        keyUp: "actionB",
        keyInput: "actionC",
        longPress: "actionD"
    }
```  
単一の操作でも複数に分かれたイベントが発生することがある点に注意してください。  
例えば、キーの押下では、押したときにkeyDownイベントが発生、放したときにkeyUpイベントと、keyInputイベントが発生します。押している時間が長い場合にはkeyInputの代わりにlongPressイベントが発生します。   
同様に、ジョイスティックを傾けて戻す一連の操作の過程では、beginRotateイベント、rotateイベント、endRotateイベントが発生します。  
```javascript
    joystick: {
        beginRotate: "actionX",
        rotate: "actionY",
        endRotate: "actionZ"
    }
```

一つのイベントに対して、カンマ区切りで複数のアクションを割り当てることもできます。
```javascript
    joystick: {
        beginRotate: "actionX",
        rotate: "actionY1, actionY2",
        endRotate: "actionZ"
    }
```
この例の場合、ジョイスティックを傾けた時にactionXが実行され、傾き加減が変化する都度actionY1に続いてactionY2が実行され、中立に戻るとactionZが実行されます。  
  
ひとつの操作部に対して複数のアクションを割り当てた場合、表示部に表示される情報はどれかひとつのアクションに定義されるvisual属性になります。  
どのアクションのvisual属性が表示されるのかは、デバイスが定義するイベントの定義順、bindsで指定するアクションの並び順に検索し、最初にvisualが定義されているアクションのものになります。  
例えば、上記の定義でactionY1とactionY2にvisual属性が定義されていれば、表示されるのはactionY1のvisual属性となります。  

発生するイベントはデバイスの種類や操作部により異なります。  
発生するイベントはデバイス定義ファイル（device/matrix.js等）で確認してください。  

## action
actionは、どのような動作を行うのかをコマンドで定義します。  
各コマンドには固有のオプションがあり、オプションの定義により詳細の動作を指定します。  
例えば、キー入力動作となる`Command.KeyInput`の場合では、`value`オプションによりどのキーを押すのかを指定します。
```javascript
    EditBlend: {
        command: Command.KeyInput,
        value: ["a"],
        :
    }
```
オプションは`value: ["a"]`のように文字を定数で指定することもできますが、スティックの傾き角度などデバイスの状態を名前で指定して参照することもできます。
```javascript
    EditBlend: {
        command: Command.MouseMove,
        x: Device.Joystick.x,
        :
    }
```
`Device.Joystick.X`はデバイスのスティックの横方向の傾き角度を示し、中立では0、右に傾けると5.13、8.43、と増加して最大で12.0までの値を取り、左に傾けると-3.54、-8.56と減少して最小で-12.0になります。  
どのような名前を使用することができるのかは、デバイス定義ファイル（device/matrix.js等）で確認してください。  

### イベントマッピングアクション  
ひとつのアクション定義内で、複数のイベントに対応する動作を定義することもできます。
イベントマッピングアクションを定義するためには、`event_`に続きマップするイベント名を記述します。
```javascript
    MoveAction: {
        event_beginMove: {
            command: Command.MouseTrackingStart
        },
        event_move: {
            command: Command.MouseMove
        },
        event_endMove: {
            command: Command.MouseTrackingRewind
        }
    }
```
このようなアクション定義の場合、イベント別のバインドはできず、指定したイベントが発生する操作部にバインドする必要があります。
例えば、上の定義例 ”MoveAction"アクションをkeySwitch1にバインドしてもなにも実行されませんが、stickにバインドすると、スティックの傾きに応じてマウスカーソルが移動し、スティックを中立に戻したときにマウスカーソルは元の位置に戻ります。

### 複数の動作を定義するアクション
一つのアクションに対して、複数の動作を定義することができます。
```javascript
    MultiAction: {
        commands: [
            {
                command: Command.KeyPress,
                value: [Key.SHIFT]  
            },{
                command: Command.MouseMove,
                x: 100,
                y: 0,
            },{
                command: Command.KeyRelease,
                value: [Key.SHIFT]
            }
        ]
    }
```
このアクションを実行すると、シフトキーを押し、マウスカーソルを右へ100移動し、シフトキーを放す動作が実行されます。  
同様に、イベントマッピングアクションでも複数アクションを記述することができます。
```javascript
    MoveAction: {
        event_beginMove: {
            commands: [
                {
                    command: Command.MouseTrackingStart
                },{
                    command: Command.KeyPress,
                    value: [Key.SHIFT]
                },{
                    command: Command.ButtonPress,
                    value: [Button.Middle]
                }
            ]
        },
        event_move: {
            command:Command.MouseMove
        },
        event_endMove: {
            commands:[
                {
                    command: Command.KeyRelease,
                    code: [Key.SHIFT]
                },{
                    command: Command.MouseTrackingRewind
                }
            ]
        }
    }
```

## コマンドの詳細
### Command.MouseMove  
マウスカーソルを移動します。
```javascript
    {
        command: Command.MouseMove,
        x: Device.Joystick.X,
        y: Device.Joystick.Y,
        s: 200,    
        xr: 1,
        yr: 1,
        r: 0
    }
```
- x オプション  
  水平方向の移動量を指定します。省略時は`Device.Joystick.X`となります。  
- y オプション  
  垂直方向の移動量を指定します。省略時は`Device.Joystick.Y`となります。
- s オプション  
  移動速度を指定します。省略時は200となります。
- xr オプション  
  水平方向の移動量に対する倍率を数値で指定します。省略時は1となります。  
  0.5を指定した場合、移動量は半分になり、-1を指定すると、移動方向が反転します。
- yr オプション  
  垂直方向の移動量に対する倍率を数値で指定します。省略時は1となります。  
  0.5を指定した場合、移動量は半分になり、-1を指定すると、移動方向が反転します。
- r オプション  
  移動方向を回転させる角度を度で指定します。省略時は0度となり回転しません。    
  正の値を指定すると反時計回りに回転し、負の値を指定すると時計回りに回転します。  
  例えば、r = 45を指定した場合でx=10, y=0と右に10移動する状態では、45度反時計回りに回転するので、右上に10移動するようになります。
### Command.MouseTrackingStart
マウスポインタの移動追跡を開始します。  
```javascript
    {
        command: Command.MouseTrackingStart
    }
```
Command.MousMoveにより移動するマウスポインタの位置を追跡し、現在の位置からの相対的な移動位置の積算を開始します。  
Command.MouseTrackingRewindにより、積算された移動量の反対方向へマウスカーソル移動させることができ、結果的にAction.MouseTrackingStartを実行したときの位置へ、マウス位置を戻すことができます。  
但し、Windows環境では必ずしもUSBマウスの移動カウントだけマウスカーソルが正確に移動するわけではないので、元の位置に完全に戻ることはなく、近い位置に移動することになります。

Command.MouseTrackingStart実行時に、既に追跡が開始されていた場合、積算されている移動量はリセットされます。
### Command.MouseTrackingStop
マウスポインタの追跡を停止します。  
```javascript
    {
        command: Command.MouseTrackingStop
    }
```

### Command.MouseTrackingRewind
マウスポインタの追跡を停止し、積算された移動量の反対方向へマウスポインタを移動します。  
```javascript
    {
        command: Command.MouseTrackingRewind
    }
```

### Command.MouseWheel
マウスホイールを回転します。
```javascript
    {
        command: Command.MouseWheel,
        delta: Device.Wheel.Delta,
        r: 1
    }
```
- delta オプション  
  ホイールの回転量を整数値で指定します。省略時は`Device.Wheel.Delta`となります。
- r オプション  
  移動量の倍率を数値で指定します。省略時は1となります。  
  2を指定した場合、回転量は倍になり、-1を指定すると、回転方向が反転します。

### Command.Click
マウスボタンをクリックします。
```javascript
    {
        command: Command.Click,
        button: Button.Left
    }
```
- button オプション  
  クリックするマウスボタンを指定します。省略時は`Button.Left`となります。

### Command.DoubleClick
マウスボタンをダブルクリックします。
```javascript
    {
        command: Command.DoubleClick,
        button: Button.Left
    }
```
- button オプション  
  クリックするマウスボタンを指定します。省略時は`Button.Left`となります。

### Command.TripleClick
マウスボタンをトリプルクリックします。
```javascript
    {
        command: Command.TripleClick,
        button: Button.Left
    }
```
- button オプション  
  クリックするマウスボタンを指定します。省略時は`Button.Left`となります。

### Command.ButtonPress
マウスのボタンを押します。
```javascript
    {
        command: Command.ButtonPress,
        button: Button.Left
    }
```
- button オプション  
  押すボタンを指定します。省略時は`Button.Left`となります。

### Command.ButtonRelease
マウスのボタンを放します。
```javascript
    {
        command: Command.ButtonRelease,
        button: Button.Middle
    }
```
- button オプション  
  放すボタンを指定します。省略時は`Button.Left`となります。

### Command.KeyPress
キーを押します。
```javascript
    {
        command: Command.KeyPress,
        value: [Key.Shift, Key.F1],
        interval: 200
    }
```
- value オプション  
  押すキーのコードを配列で指定します。省略はできません。
- interval オプション  
  valueに複数のコードを指定した場合、キーを押す間隔をミリ秒で指定します。省略時は50となります。
 
### Command.KeyRelease
押したキーを放します。
```javascript
    {
        command: Command.KeyRelease,
        value: [Key.F1, Key.Shift]
        interval: 200
    }
```
- value オプション  
  放すキーのコードを配列で指定します。省略はできません。  
- interval オプション  
  valueに複数のコードを指定した場合、キーを放す間隔をミリ秒で指定します。省略時は50となります。  

### Command.KeyInput
文字をキー入力します。  
keyDownやkeyReleaseとは異なり、コードだけではなく入力する文字を直接指定することができ、キースイッチを押して放す動作が実行されます。  
このアクション終了時、押されたままになっているモディファイアキー(Key.CTRL, Key.SHIFT, Key.ALT, Key.META)がある場合、すべて放されます。
```javascript
    {
        command: Command.KeyInput,
        value: ["ABC123xyz", Key.RETURN],
        interval: 200
    }
```
- value オプション  
  入力する文字列かコードの配列を指定します。省略はできません。  
  文字列は、デバイスの定義により英語配列または日本語配列のキーボードの仕様に従ってキーコードに変換され、押して放す動作が実行されます。  
  記号やアルファベットの大文字など、シフトキーを併用する文字の場合、シフトキーと文字キーの操作に分解されて実行されます。例えば、"Ab"を指定した場合次のような操作が実行されます。
  - Key.SHIFTを押す
  - Key.Aを押す
  - Key.Aを放す
  - Key.SHIFTを放す
  - Key.Bを押す
  - Key.Bを放す  

  value中にモディファイアキー(Key.CTRL, Key.SHIFT, Key.ALT, Key.META)を指定した場合、トグル動作となります。  
  例えば、`[Key.SHIFT, "a", Key.SHIFT, "b"]`では、最初のKey.SHIFTでシフトキーが押され、"a"でKey.Aを押して放し、2番目のKey.SHIFTでシフトキーが放され、"b"でKey.Bを押して放します。その結果、入力される文字列は"Ab"となります。  
  記号やアルファベット大文字の入力におけるシフトキー操作は、明示的にvalueに指定するKey.SHIFTや、先行するkeyPressアクションによるシフトキーの押下状態は考慮されません。  
  従って、`[Key.SHIFT, "Xy"]`では、次のような操作が実行されます。
  - Key.SHIFTを押す (明示的な操作)
  - Key.SHIFTを放す (Xが大文字のため自動挿入され、トグル動作により放す動作となる)
  - Key.Xを押す
  - Key.Xを放す
  - Key.SHIFTを押す (自動挿入されたシフトキーを放す操作が、トグル動作により押す操作になる)
  - Key.Yを押す
  - Key.Yを放す  

  これらの操作により実際に入力される文字は"xY"となります。
- interval オプション  
  キーを操作する間隔をミリ秒で指定します。省略時は50となります。  

※ 入力できる文字はキーボードで直接入力できる文字に限定されます。日本語などIMEにより生成される文字を入力することはできません。

### Command.MapInput
オプションの値に応じて、キー入力します。
```javascript
    {
        command: Command.MapInput,
        key: Device.Joystick.Z,
        map: [
            {le:-8, value: [Key.CTRL, Key.SHIFT, "9"]},   
            {le:-4, value: [Key.CTRL, Key.SHIFT, "8"]},   
            {le:-1, value: [Key.CTRL, Key.SHIFT, "7"]},   
            {ge:8,  value: [Key.CTRL, "9"]},   
            {ge:4,  value: [Key.CTRL, "8"]},   
            {ge:1,  value: [Key.CTRL, "7"]}
        ],   
        continuous: true,
        interval: 200
    }
```
- key オプション  
  マップを選択するキーとなる値を指定します。省略できません。
- map オプション  
  keyの値に応じて実行するキー入力操作を配列で指定します。 省略できません。   
  keyの値との比較は、lt（未満）, le（以下）、eq（等しい）、ge（以上）、gt（より大きい）属性で比較方法と値を指定します。  
  配列の先頭から比較を行い、最初に条件を満たした要素の操作が実行されます。従って、上記の例の定義で、スティックが2.5だけ捻られているとき、
  配列の最後の要素`ge: 1`が満たされるため、CTRL+7がキー入力されます。  
  同様に、スティックを10捻った場合は、配列の4番目`ge:8`が満たされ、CTRL+9がキー入力されます。
- continuous オプション  
  mapの条件を満たしている間、キーを押したしたままにする場合はtrueを指定します。押されたキーは条件が満たされなくなった時に放されます。  
  trueを指定しない場合、通常のキー操作と同様に押して放す操作が実行され、codeの値が変化しても他の配列要素の条件を満たすまでキー入力は行われません。
- interval オプション  
  valueに複数のスキャンコードを指定した場合やcontinuousオプションにtrueを指定した場合、複数のキー操作が実行されますが、その操作間隔をミリ秒で指定します。省略時は50となります。

### Command.MapAction
オプションの値に応じて、actionを実行します。

```javascript
    {
      command: Command.MapCommand,
      key:  Device.Joystick.Z,
      map:  [
        {
          le: 0,
          commands: [
            {
              command: Command.MouseMove,
              x: -100
            }
          ]
        },
        {
          commands: [
            {
              command: Command.MouseMove,
              x: 100
            }
          ]
        }
      ]
    }
```
- key オプション  
  マップを選択するキーとなる値を指定します。省略できません。
- map オプション  
  keyの値に応じて実行するアクションを配列で指定します。 省略できません。   
  keyの値との比較は、lt（未満）, le（以下）、eq（等しい）、ge（以上）、gt（より大きい）、ne（不一致）属性で比較方法と値を指定します。  
  配列の先頭から比較を行い、最初に条件を満たした要素の操作が実行されます。  
  条件を記述しない場合、比較は常に成功します。従って、map配列の最後に条件を記述しない要素を配置することで、先行する条件を満たさない場合に実行するアクションを定義することができます。

### Command.PageChange
bindsページを切り替えます。
```javascript
    {
        command: Command.PageChange,
        page: "Mode3D"
    }
```
- page オプション  
  切り替えるページの名前を指定します。省略時は定義順で次のページに切り替えます。  
### Command.ApplicationChange
アプリケーションを切り替えます。
```javascript
    {
        command: Command.ApplicationChange,
        application: "DSM",
        page: "Mode3D"
    }
```
- application オプション  
  切り替えるアプリケーションの名前を指定します。 省略時は定義順で次のアプリケーションに切り替えます。 
- page オプション  
  切り替えたアプリケーションのどのページを表示するのかを指定します。省略時は定義順で先頭のページが表示されます。

### Command.Delay
一定時間アクションの実行を停止します。  
複数のアクションを連続して実行する場合、先行するアクションの実行後、後続のアクションの実行を開始するまでに一定の時間を待機したい場合があります。
Command.Delayを間に挟むことによって、後続のアクションの実行を遅延させることができます。  
```javascript
    {
        command: Command.Delay,
        timeout: 800
    }
```
- timeout オプション  
  遅延させる時間をミリ秒で指定します。省略はできません。

### Command.SetTimer
各操作部に対してタイマーをセットします。  
指定時間が経過すると、timeoutイベントが発生します。  
timeoutイベントは1回のSetTimerに対して1回だけ発生し、繰り返し発生することはありません。
```javascript
    {
        command: Command.SetTimer,
        target: "key0",
        timeout: 800,
        data: 1
    }
```
- target オプション  
  タイマーをセットする操作部の名前を指定します。省略はできません。
- timeout オプション  
  タイマーの時間をミリ秒で指定します。省略はできません。
- data オプション  
  タイマーに付与する任意のデータを指定します。

一つの操作部にセットできるタイマーは一つだけです。SetTimerを実行時にい既にセットされているタイマーは無効になります。
