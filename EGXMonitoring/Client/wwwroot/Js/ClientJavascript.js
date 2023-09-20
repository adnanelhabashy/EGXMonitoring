window.enterFullScreen = function (elementId) {
    const element = document.getElementById(elementId);
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.webkitRequestFullscreen) {
        element.webkitRequestFullscreen();
    } else if (element.msRequestFullscreen) {
        element.msRequestFullscreen();
    }
};

window.exitFullScreen = function () {
    if (document.exitFullscreen) {
        document.exitFullscreen();
    } else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
    } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
    } else if (document.msExitFullscreen) {
        document.msExitFullscreen();
    }
};

function toast(title, subtitle, body) {
    const toastOptions = {
        class: 'bg-danger',
        title: title,
        subtitle: subtitle,
        body: body,
        position: 'bottomLeft',
        autohide: true,
        delay: 10000,
    };
    $(document).Toasts('create', toastOptions);
}


function toastDanger(title, subtitle, body) {
    const toastOptions = {
        class: 'bg-danger',
        title: title,
        subtitle: subtitle,
        body: body,
        position: 'bottomLeft',
        autohide: true,
        delay: 10000,
    };
    $(document).Toasts('create', toastOptions);
}
function toastSuccess(title, subtitle, body) {
    const toastOptions = {
        class: 'bg-success',
        title: title,
        subtitle: subtitle,
        body: body,
        position: 'bottomLeft',
        autohide: true,
        delay: 10000,
    };
    $(document).Toasts('create', toastOptions);
}
