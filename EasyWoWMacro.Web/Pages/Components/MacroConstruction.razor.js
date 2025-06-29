export function initializeDropHandlers(dotNetHelper) {
    console.log('Initializing drop handlers for macro construction...');
    
    // Set up drop handlers for all existing lines
    setupDropHandlersForAllLines(dotNetHelper);
}

export function setupDropHandlersForAllLines(dotNetHelper) {
    // Find all line content elements
    const lineContents = document.querySelectorAll('.line-content');
    console.log(`Found ${lineContents.length} line content elements`);
    
    lineContents.forEach((element, index) => {
        setupDropHandler(element, index, dotNetHelper);
    });
}

function setupDropHandler(element, lineIndex, dotNetHelper) {
    console.log(`Setting up drop handler for line ${lineIndex}`);
    
    // Remove any existing drag event listeners by cloning and replacing
    // But preserve the original element to keep Blazor events
    const existingDragEvents = element.getAttribute('data-drag-handlers-setup');
    if (existingDragEvents === 'true') {
        console.log(`Drop handlers already set up for line ${lineIndex}`);
        return;
    }
    
    // Set up dragover event
    element.addEventListener('dragover', function(e) {
        e.preventDefault();
        e.stopPropagation();
        e.dataTransfer.dropEffect = 'copy';
        console.log(`Dragover on line ${lineIndex}`);
    });
    
    // Set up drop event
    element.addEventListener('drop', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        const blockType = e.dataTransfer.getData('text/plain');
        console.log(`Drop detected on line ${lineIndex} with block type: ${blockType}`);
        
        // Call the .NET method
        dotNetHelper.invokeMethodAsync('OnDropWithData', lineIndex, blockType);
    });
    
    // Set up dragenter event
    element.addEventListener('dragenter', function(e) {
        e.preventDefault();
        e.stopPropagation();
        console.log(`Drag enter on line ${lineIndex}`);
        this.classList.add('drag-over');
    });
    
    // Set up dragleave event
    element.addEventListener('dragleave', function(e) {
        e.preventDefault();
        e.stopPropagation();
        console.log(`Drag leave on line ${lineIndex}`);
        this.classList.remove('drag-over');
    });
    
    // Mark that drag handlers have been set up
    element.setAttribute('data-drag-handlers-setup', 'true');
}

export function getDragData() {
    console.log('Getting drag data from global variable');
    // Import the function from BuildingBlocks module
    return window.currentDragBlockType;
} 