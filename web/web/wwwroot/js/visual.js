function shakeonclick(id) {
    element = document.getElementById("subcard-" + id);
    element.style.animation = "shake .3s";

    element.addEventListener("animationend", (e) => { element.style.animation = ""; })
}