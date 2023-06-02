import { ControlValue } from "@shared/models/forms/control-value";
import { Picture } from "@shared/models/pictures/picture";
import { PersonEditorData } from "./person-editor-data";

export interface PersonSaveFormData {
    personEditorInfo: PersonEditorData;
    formValues: ControlValue[];
    updatedPhotos: Picture[];
    removedPhotos?: number[];
    addedPhotos: Picture[];
}