// Global variable to store the current block type being dragged
let currentDragBlockType = null;

export function setCurrentDragBlockType(blockType) {
    console.log('Setting current drag block type:', blockType);
    currentDragBlockType = blockType;
    // Also set it on window for cross-module access
    window.currentDragBlockType = blockType;
}

export function getCurrentDragBlockType() {
    return currentDragBlockType;
}

export function clearCurrentDragBlockType() {
    currentDragBlockType = null;
    window.currentDragBlockType = null;
}

export function setDragData(elementId, blockType) {
    console.log('Setting drag data for block type:', blockType);
    
    // Find the element and get its native drag event
    const element = document.getElementById(elementId);
    if (element) {
        // Set up the drag start event on the element
        element.addEventListener('dragstart', function(e) {
            console.log('Native drag start for block type:', blockType);
            
            // Set the drag data
            e.dataTransfer.effectAllowed = 'copy';
            e.dataTransfer.setData('text/plain', blockType);
            
            // Add visual feedback
            this.style.opacity = '0.5';
        });
        
        element.addEventListener('dragend', function(e) {
            // Remove visual feedback
            this.style.opacity = '1';
        });
    } else {
        console.warn(`Element with ID ${elementId} not found`);
    }
}

// Store the current drag event globally
export function storeDragEvent(dragEvent) {
    window.currentDragEvent = dragEvent;
}

export function initializeDragHandlers() {
    console.log('Initializing drag handlers for building blocks...');
    
    // Set up drag handlers for each building block type
    setupDragHandler('directive-block', 'Directive');
    setupDragHandler('command-block', 'Command');
    setupDragHandler('conditional-block', 'ConditionalGroup');
    setupDragHandler('argument-block', 'Argument');
}

function setupDragHandler(elementId, blockType) {
    const element = document.getElementById(elementId);
    if (element) {
        console.log(`Setting up drag handler for ${elementId} with block type ${blockType}`);
        
        // Remove any existing event listeners
        const newElement = element.cloneNode(true);
        element.parentNode.replaceChild(newElement, element);
        
        // Set up drag start event
        newElement.addEventListener('dragstart', function(e) {
            console.log(`Drag start for ${blockType}`);
            
            // Set the drag data using native DataTransfer
            e.dataTransfer.effectAllowed = 'copy';
            e.dataTransfer.setData('text/plain', blockType);
            
            // Add visual feedback
            this.style.opacity = '0.5';
        });
        
        // Set up drag end event
        newElement.addEventListener('dragend', function(e) {
            console.log(`Drag end for ${blockType}`);
            
            // Remove visual feedback
            this.style.opacity = '1';
        });
    } else {
        console.warn(`Element with ID ${elementId} not found`);
    }
} 