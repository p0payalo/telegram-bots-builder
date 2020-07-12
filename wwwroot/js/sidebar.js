var items = $(".sidebar-item");
var views = $(".sidebar_view");
for (let i = 0; i < items.length; ++i)
{
    items[i].addEventListener("click", function () {
        let active = $(".active")[0];
        let active_view = $(".active_view")[0];
        active.classList.remove("active");
        active_view.classList.remove("active_view");
        items[i].classList.add("active");
        views[i].classList.add("active_view");
    }, false);
}

var toggle = $(".toggle-power");
for (let i = 0; i < toggle.length; ++i) {
    toggle[i].addEventListener("click", function (e) {
        e.preventDefault();
        var action = this.getAttribute("href");
        window.location.href = action;
        if (action.includes("start")) {
            action = action.replace("start", "stop");
            this.innerText = "Stop Bot";
            this.classList.remove("btn-success")
            this.classList.add("btn-danger");
        }
        else {
            action = action.replace("stop", "start");
            this.innerText = "Start Bot";
            this.classList.remove("btn-danger")
            this.classList.add("btn-success");
        }
        this.setAttribute("href", action);

    });
}
