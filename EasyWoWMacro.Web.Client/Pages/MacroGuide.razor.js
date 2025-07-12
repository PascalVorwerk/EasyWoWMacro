export function scrollToSection(sectionId) {
    const element = document.getElementById(sectionId);
    console.log(`Scrolling to section: ${sectionId}`, element);
    
    if (element) {
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    }
};