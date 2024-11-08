// MacroEditor.razor.js

export function initializeDropArea(dropAreaId) {
    const dropArea = document.getElementById(dropAreaId);

    dropArea.addEventListener("dragover", (event) => {
        event.preventDefault();
        event.dataTransfer.dropEffect = "copy";
    });

    dropArea.addEventListener("drop", (event) => {
        event.preventDefault();
        event.dataTransfer.dropEffect = "copy";
    });
}
