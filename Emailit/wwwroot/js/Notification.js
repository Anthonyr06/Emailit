"use strict";
var notificationConn = new signalR.HubConnectionBuilder()
    .withUrl("/NotificationHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Debug)//signalR.LogLevel.None
    .build();

notificationConn.serverTimeoutInMilliseconds = 100000; // 100 second. Consider removing in production

notificationConn.on("sendNotification", (data) => {
    console.log("SignalR: " + data);

    var notification = $('<div class="fade show toast mb-2" role="alert" aria-live="assertive" aria-atomic="true">' +
        '  <div class="toast-header">' +
        '    <div class="bg-success mr-1 rounded-circle p-1"></div>' +
        '    <strong class="mr-auto text-truncate">' + data.tittle + '</strong>' +
        '    <small class="ml-auto text-truncate" >' + data.author + '</small>' +
        '    <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">' +
        '      <span aria-hidden="true">×</span>' +
        '    </button>' +
        '  </div>' +
        '  <div class="toast-body">' + data.content.substring(0, 140) +
        '  </div>' +
        '</div>');

    $('#notificationsContainer').append(notification);

    updateInboxReceivedBadge();

    //Add new message to the message list, only if the user is on the received page
    if (currentView == 1) {
    addNewReceivedMsg(data);
    } 

    

});

notificationConn.start().catch(function (err) {
    return console.error(err.toString());
});

$('body').on('click', '.close', function () {
    $(this).closest('.toast').toast('hide')
})