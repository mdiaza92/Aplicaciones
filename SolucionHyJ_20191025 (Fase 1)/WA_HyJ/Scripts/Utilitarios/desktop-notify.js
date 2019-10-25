function createNotification(title, icon, body, href, tag, timeout, language, direction, data) {
    if (!Notification) {
        alert('Las notificaciones por escritorio no son soportadas por esta versión del navegados. Intenta con Chrome.');
        return;
    }
    else {
        var options = {
            body: body,
            icon: icon,
            tag: tag,
            timeout: timeout,
            lang: language,
            dir: direction, //ltr, rtl o auto
            data: data,
            vibrate: [200, 100, 200],
            lang: 'es-PE'
        };

        if (Notification.permission === "granted") {
            var notification = new Notification(title, options);
            notification.vibrate;

            if (href != null) {
                notification.onclick = function () {
                    window.open(href);
                };
            }

            notification.onerror = function () {
                alert("Se produjo un error al mostrar la notificación.");
            };

        }
        //else if (Notification.permission !== 'denied') {
        //    Notification.requestPermission(function (permission) {
        //        if (permission === "granted") {
        //            var notification = new Notification(title, options);
        //            if (href != null) {
        //                notification.onclick = function () {
        //                    window.open(href);
        //                };
        //            }
        //        }
        //    });
        //}
    }
}
