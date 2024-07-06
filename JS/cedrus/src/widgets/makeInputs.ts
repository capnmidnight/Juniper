import { InputCheckbox, InputDate, InputFile, InputNumber, InputText, Option, PlaceHolder, Select, Step, TitleAttr, Value, ValueAsDate, ValueAsNumber } from "@juniper-lib/dom";
import { SelectPlaceholder } from "@juniper-lib/widgets";
import { PropertyModel, PropertyTypeModel, isPropertyModel, isPropertyTypeModel } from "../models";

export function makeInputForPropertyType(type: PropertyTypeModel, validValuesByPropertyTypeId: Map<number, string[]>): HTMLInputElement | HTMLSelectElement {
    if (isPropertyTypeModel(type, "String") && validValuesByPropertyTypeId.has(type.id)) {
        const values = validValuesByPropertyTypeId.get(type.id);
        return Select(
            SelectPlaceholder(type.name),
            TitleAttr(type.name),
            ...values.map(v => Option(v))
        );
    }

    if (isPropertyTypeModel(type, "Boolean")) {
        return InputCheckbox(
            TitleAttr(type.name)
        );
    }

    if (isPropertyTypeModel(type, "Date")) {
        return InputDate(
            PlaceHolder(type.name),
            TitleAttr(type.name)
        );
    }

    if (isPropertyTypeModel(type, "Decimal") || isPropertyTypeModel(type, "Integer")) {
        return InputNumber(
            PlaceHolder(type.name),
            TitleAttr(type.name),
            Step(type.dataType === "Decimal" ? 0.01 : 1)
        );
    }

    if (isPropertyTypeModel(type, "File")) {
        return InputFile(
            TitleAttr(type.name)
        );
    }

    return InputText(
        PlaceHolder(type.name),
        TitleAttr(type.name)
    );
}

export function makeInputForProperty(property: PropertyModel, validValuesByPropertyTypeId: Map<number, string[]>): HTMLInputElement | HTMLSelectElement {

    if (isPropertyModel(property, "String") && validValuesByPropertyTypeId.has(property.type.id)) {
        const values = validValuesByPropertyTypeId.get(property.type.id);
        return Select(
            SelectPlaceholder(property.type.name),
            TitleAttr(property.type.name),
            ...values.map(v => Option(v)),
            Value(property.value)
        );
    }

    if (isPropertyModel(property, "Boolean")) {
        return InputCheckbox(
            TitleAttr(property.type.name),
            Value(property.value ? "on" : "off")
        );
    }

    if (isPropertyModel(property, "Date")) {
        return InputDate(
            PlaceHolder(property.type.name),
            TitleAttr(property.type.name),
            ValueAsDate(property.value)
        );
    }

    if (isPropertyModel(property, "Decimal") || isPropertyModel(property, "Integer")) {
        return InputNumber(
            PlaceHolder(property.type.name),
            TitleAttr(property.type.name),
            Step(property.type.dataType === "Decimal" ? 0.01 : 1),
            ValueAsNumber(property.value)
        );
    }

    if (isPropertyModel(property, "File")) {
        return InputFile(
            TitleAttr(property.type.name)
        );
    }

    return InputText(
        PlaceHolder(property.type.name),
        TitleAttr(property.type.name),
        Value(property.value)
    );
}

export function getPropertyValueFromInput(input: HTMLInputElement | HTMLSelectElement) {

    if (input instanceof HTMLSelectElement) {
        return input.value;
    }

    if (input.type === "checkbox") {
        const expectedValue = input.name || "on";
        return input.value == expectedValue;
    }

    if (input.type === "date") {
        return input.valueAsDate;
    }

    if (input.type === "number") {
        return input.valueAsNumber;
    }

    if (input.type === "file") {
        return input.files;
    }

    return input.value;
}