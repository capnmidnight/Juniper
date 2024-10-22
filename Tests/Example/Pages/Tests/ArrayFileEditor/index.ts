import { identity } from "@juniper-lib/util";
import { CedrusDataAPI, FileModel, isFileModel } from "@juniper-lib/cedrus";
import { Accept, Article, Button, Multiple, OnClick, Query } from "@juniper-lib/dom";
import { FileInput, FileInputElement, FileViewElement, IBasicFile, OnRequestInput } from "@juniper-lib/widgets";
import "./index.css";

const fileInput = FileInput(
    Multiple(true),
    Accept("image/*"),
    OnRequestInput<IBasicFile, string>(evt => {
        if (isFileModel(evt.value)) {
            evt.resolve(evt.value.path);
        }
        else {
            evt.reject();
        }
    })
);

Article(Query("article"),
    Button("Save", OnClick(async () => {
        const files = await save(fileInput);
        fileInput.clear();
        fileInput.addFiles(files);
    })),
    fileInput
);

const ds = await CedrusDataAPI.dataSourceTask;
fileInput.addFiles(await ds.getFiles());

async function save(input: FileInputElement) {
    const views = Array.from(input.fileViews);
    const fileModels = new Map<FileViewElement, FileModel>();
    const toSave = new Array<FileViewElement>();
    for (const view of views) {
        if (!view.deleting) {
            if (isFileModel(view.file)) {
                fileModels.set(view, view.file);
            }
            else if (view.file instanceof File) {
                toSave.push(view);
            }
        }
    }

    const savedFiles = await ds.uploadFiles(toSave.map(f => f.file as File));
    for (let i = 0; i < toSave.length; ++i) {
        fileModels.set(toSave[i], savedFiles[i]);
    }

    const models = views
        .map(v => fileModels.get(v))
        .filter(identity);

    return models;
}

