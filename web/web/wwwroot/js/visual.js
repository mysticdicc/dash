function shakeonclick(id) {
    element = document.getElementById("subcard-" + id);
    element.style.animation = "shake .3s";

    element.addEventListener("animationend", (e) => { element.style.animation = ""; })
}

window.initMasonry = function(selector, options) {
    var elem = document.querySelector(selector);
    if (!elem) {
        return;
    }
    
    var items = elem.querySelectorAll(options.itemSelector);
    
    if (elem.masonryInstance) {
        elem.masonryInstance.destroy();
    }
    
    elem.masonryOptions = options;
    elem.masonryInstance = new Masonry(elem, options);
};

window.relayoutMasonry = function(selector) {
    var elem = document.querySelector(selector);
    if (!elem) {
        return;
    }
    
    var items = elem.querySelectorAll(".settings_card");
    
    if (elem.masonryInstance) {
        elem.masonryInstance.destroy();
    } else {
    }
    
    var options = elem.masonryOptions || {
        columnWidth: 400,
        itemSelector: ".settings_card",
        gutter: 10,
        fitWidth: true
    };
    
    elem.masonryInstance = new Masonry(elem, options);
};

window.destroyMasonry = function(selector) {
    var elem = document.querySelector(selector);
    if (elem && elem.masonryInstance) {
        elem.masonryInstance.destroy();

    } else {
    }
};