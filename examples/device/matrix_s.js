import {PathUtil, PixelFormat} from "./aergia_types.js"
import { Matrix } from './matrix.js';

let Matrix_s = JSON.parse(JSON.stringify(Matrix));
for (let i = 0; i < 16; i++) {
    Matrix_s.controls["KeySwitch" + i].visualCapabilities.icon = {
        pixelFormat: PixelFormat.Rgb565,
        resolution: {
            h: 46, v: 22
        }
    };
}
Matrix_s.model = "Aergia Matrix S";

export {Matrix_s};
