import { CanvasTypes } from "@juniper-lib/dom/dist/canvas";
import type { IProgress } from "@juniper-lib/progress/dist/IProgress";
export declare function equirectangularToCubemapBlobs(image: TexImageSource, isStereo: boolean, size: number, prog?: IProgress): Promise<Blob[]>;
export declare function equirectangularToCubemapCanvases(image: TexImageSource, isStereo: boolean, size: number, prog?: IProgress): Promise<CanvasTypes[]>;
//# sourceMappingURL=equirectangularToCubemap.d.ts.map