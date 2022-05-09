
let MessageIdContextMenu = 0;
let controller = new AbortController();

//in this variable the necessary information is stored in order to know if we are in the received(1) or sent(2) view.
let currentView = 0; 

// It will help us to know if the end of the list of messages has already been reached, without even having loaded new messages.
var bottomMessageListHasBeenReached = false; 

let currentPage = 1;// This is the page where the user is currently on.
let hasNext = false;//Indicates if it is available to go to the next page.
let search = ''; //Variable to search for messages
let IdOfMessageToDelete = 0; 

function deleteMsg() {

    fetch('/Message/Delete', {
        method: 'POST',
        body: new URLSearchParams({
            messageId: IdOfMessageToDelete,
            currentView: currentView
        }),
        headers: {
            RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        redirect: "manual",
        signal: controller.signal
    })
        .then((res) => {
            if (!res.ok) throw new Error(res.error);
            return res.text();
        })
        .then(() => {
            let divMsgId = ('#msg-list-item-' + IdOfMessageToDelete);

            $(divMsgId).removeClass('animate__animated animate__fadeInUpBig');
            closeOpenedMsg();
            animateCSS(divMsgId, 'backOutLeft').then((message) => {
                $(document).find(divMsgId).remove();
            });
        })
        .catch(error => {
            //si no fue cancelada por el metodo AbortController()
            if (error.code !== DOMException.ABORT_ERR) {
                console.log(error);
                alert('Error!! Refresh the page.');
            }
        })
        .finally(function () {
            IdOfMessageToDelete = 0;

            updateInboxReceivedBadge();
        });

}

function searchMsg(val) {
    if (val.length > 0 & val.length <= 100) {

        search = val;

        closeOpenedMsg();
        CancelAllFetch();
        LoadingAnimationInbox('#msgList', true);
        if (currentView == 1) {
            ReceivedInbox(true);
        } else if (currentView == 2) {
            SentInbox(true);
        }

        $('#clearSearchMsgBtn').show('slow');
    }
}

function clearSearchMsg() {
    $('#searchMsgInputTxt').val('');
    $('#clearSearchMsgBtn').hide();
    search = '';
}

//Cancel all Fetches that are implementing the AbortSignal
function CancelAllFetch() {
    controller.abort();
    controller = new AbortController();
}


//Load received messages
function ReceivedInbox(clean, page) {
    $('#TitleMsgListCard > #TitleMsgListCard1').show();
    $('#TitleMsgListCard > #TitleMsgListCard2').hide();

    bottomMessageListHasBeenReached = true;

    fetch('/Message/Received?' + new URLSearchParams({
        PageNumber: page,
        Search: search
    }), { redirect: "manual", signal: controller.signal })
        .then((res) => {

            currentPage = res.headers.get('pagination-current-page');
            hasNext = res.headers.get('pagination-has-next');

            if (!res.ok) throw new Error(res.error);

            return res.text();
        })
        .then(html => {

            if (clean) {
                $('#msgList').empty();
            }

            $('#msgList').append(html);
        })
        .catch(error => {
            //If it was not canceled by the AbortController() method
            if (error.code !== DOMException.ABORT_ERR) {
                console.log(error);
                ErrorHasOcurredFetch('#msgList');
            }
        })
        .finally(function () {

            ActiveListItemInbox("#InboxRecivedMsgBtn");
            currentView = 1;
            bottomMessageListHasBeenReached = false;

            $('#loadingIcon').remove(); //it's used to remove the loading animation when using the scroll pagination
        });

    updateInboxReceivedBadge();
}

function addNewReceivedMsg(data) {

    let attachedIco = data.itHasAttachedFiles ? '&nbsp;<span><img src="/icon/attachBtn.svg" class="mb-1" style="width:1rem;" /></span>' : '';

    let html = '<a href="#" id="msg-list-item-' + data.messageId + '" class="pb-2 list-group-item list-group-item-action custom-right-click msg-list-item animate__animated animate__fadeInUpBig" style="--animate-duration:.7s;">' +
        '		<div class="d-flex w-100 justify-content-between">' +
        '            <h6 class="text-truncate">' +
        '                    <img src="/icon/mail-closed.svg" class="mb-1" style="width:1rem;" />&nbsp;' +
                                data.tittle + attachedIco +
        '            </h6>' +
        '			<small class="text-muted" style="font-size:73%;">Right now</small>' +
        '		</div>' +
        '		<p class="block-with-text pb-1">' +
                    data.content+
        '		</p>' +
        '	</a>';


    $('#msgList').prepend(html);
    $("#emptyInbox").hide();
    updateInboxReceivedBadge();
}

function updateInboxReceivedBadge() {

    let notSeenCount = 0;

    fetch('/Message/Received?' + new URLSearchParams({
        Handler: 'Notseen'
    }), { redirect: "manual" })
        .then((res) => {
            if (!res.ok) throw new Error(res.error);
            return res.json();
        }).then(data => {
            notSeenCount = data;
		})
        .catch(error => {
                console.log(error);
        })
        .finally(function () {
            console.log('count: ' + notSeenCount);
            if (Number(notSeenCount) > 0) {
                $("#InboxRecivedMsgBtn > span.badge").html(notSeenCount);
            }
    else {
        $("#InboxRecivedMsgBtn > span.badge").hide();
    }
        });
}


//load sent messages
function SentInbox(limpiar, page) {
    $('#TitleMsgListCard > #TitleMsgListCard2').show();
    $('#TitleMsgListCard > #TitleMsgListCard1').hide();

    bottomMessageListHasBeenReached = true;

    fetch('/Message/Sent?' + new URLSearchParams({
        PageNumber: page,
        Busqueda: search
    }), { redirect: "manual", signal: controller.signal })
        .then((res) => {

            currentPage = res.headers.get('pagination-current-page');
            hasNext = res.headers.get('pagination-has-next');

            if (!res.ok) throw new Error(res.error);


            return res.text();
        })
        .then(html => {
            if (limpiar) {
                $('#msgList').empty();
            }

            $('#msgList').append(html);

        })
        .catch((error) => {
            if (error.code !== DOMException.ABORT_ERR) {
                console.log(error);
                ErrorHasOcurredFetch('#msgList');
            }
        })
        .finally(function () {
            //   $("#msgList").addClass('pre-scrollable');
            ActiveListItemInbox("#InboxSentMsgBtn");
            currentView = 2;
            bottomMessageListHasBeenReached = false;

            $('#loadingIcon').remove();
        });
}

//loading animation
function LoadingAnimationInbox(idContainer, clean) {

    let LoadingIcon = '<div id=loadingIcon class="mt-5 p-3 text-center">' +
        '<div class="bg-white w-25 mx-auto rounded-pill p-3">' +
        '<div class="spinner-border text-warning" style="width: 3rem; height: 3rem;" role="status">' +
        '<span class="sr-only">Loading...</span></div>' +
        '</div> </div>';

    //let LoadingIcon = '<div class="mx-auto rounded-pill mt-5 p-3 bg-white ">' +
    //    '<span class="spinner-grow text-warning mr-1" role="status"><span class="sr-only">Loading...</span></span>' +
    //    '<span class="spinner-grow text-warning mr-1" role="status"><span class="sr-only">Loading...</span></span>' +
    //    '<span class="spinner-grow text-warning" role="status"><span class="sr-only">Loading...</span></span></div>';

    if (clean) {
        $(idContainer).empty();
    }

    $(idContainer).append(LoadingIcon);

}

//error screen for fetch call
function ErrorHasOcurredFetch(idContainer) {

    let Error = '<div class="mt-5 p-3 text-center">' +
        '<div class="bg-white mx-auto w-75 rounded-pill p-3">' +
        '<h5 class="pl-2 pt-2">' +
        '<img src="/icon/error.svg" class="mr-2 mb-1" style="width:3rem;" />Ha ocurrido un error.</h5>' +
        '<p class="text-muted">Actualiza la pagina.</P>' +
        '</div>';
    $(idContainer).empty().append(Error);
}

//Mark the item in the left side options list as active. (Received, sent, etc.)
function ActiveListItemInbox(id) {
    $(".list-group-inbox").children().removeClass("active");
    $(".list-group-inbox > a > span > img").removeClass("white-filter");
    $(".list-group-inbox > a > span.badge").removeClass("badge-light");

    $(id).addClass("active");
    $(id + "> span > img").addClass("white-filter");
    $(id + "> span.badge").addClass("badge-light");
}

//open\read message
function OpenMsg(id) {

    $('#OpcionesInboxCol').removeClass('col-xl-2 col-md-3').addClass('col-xl-3');
    $('#TitleMsgListCard').hide();
    $('#OpenMessage').addClass('col-7 col-xl');

    LoadingAnimationInbox("#OpenMessage", true);

    fetch('/Message/Details?' + new URLSearchParams({
        id: id,
        signal: controller.signal
    }), { redirect: "manual", signal: controller.signal })
        .then((res) => {
            if (!res.ok) throw new Error(res.error);
            return res.text();
        })
        .then(html => {

            //Adding src images
            document.getElementById('OpenMessage').innerHTML = html;

            let divMessageId = ('#msg-list-item-' + id);

            $(divMessageId + '> div > h6 > img').attr("src", "/icon/mail-opened.svg");


        })
        .catch((error) => {
            //If it was not canceled by the AbortController() method
            if (error.code !== DOMException.ABORT_ERR) {
                console.log(error);
                ErrorHasOcurredFetch('#OpenMessage');
            }
        }).finally(function () {
            setFilesIconOpenedMsg();
            ActiveMessageListItem("#msg-list-item-" + id);

            updateInboxReceivedBadge();
        });

}

function setFilesIconOpenedMsg() {

    for (var i = 0; i < $('.file-from-open-msg').length; i += 1) {

        let el = $('.file-from-open-msg')[i];

        let contentType = $(el).data("contenttype");

        $('.file-from-open-msg').each(function () {

            //If the file is an image, the src of it will be put from the server
            if (!contentType.match('image')) {
                let data = getMimeTypeIconUrl(contentType);
                $(el).attr("src", data);
            }

        });

    }

}

//Mark message list item as active
function ActiveMessageListItem(id) {
    $(".msg-list-item").removeClass("active");
    $(".msg-list-item > div > h6 > img").removeClass("white-filter");
    $(".msg-list-item > div > h6 > span > img").removeClass("white-filter");
    $(".msg-list-item > div > small").removeClass("text-muted");

    $(id).addClass("active");
    $(id + "> div > h6 >img").addClass("white-filter");
    $(id + "> div > h6 > span > img").addClass("white-filter");
}

//close open message
function closeOpenedMsg() {

    animateCSS('#OpenMessage', 'slideOutDown').then((message) => {
        //When the animation ends this will be executed
        $('#OpcionesInboxCol').removeClass('col-xl-3').addClass('col-xl-2 col-md-3');
        $('#TitleMsgListCard').show();
        $('#OpenMessage').removeClass('col-7 col-xl');
        $('#OpenMessage').empty();

        ActiveMessageListItem(null);
    });
}

$(document).ready(function () {


    LoadingAnimationInbox('#msgList', true);
    ReceivedInbox(true); // load received messages

    //CUSTOM CONTEXT MENU: to reuse it just take this algorithm, use the .custom-right-click class and create an html menu
    const menu = document.querySelector(".context-menu");
    let menuTargetContainer = $('#msgList');
    let menuVisible = false;

    const setPosition = ({ top, left }) => {
        menu.style.left = `${left}px`;
        menu.style.top = `${top}px`;
        toggleMenu('show');
    };
    const toggleMenu = command => {
        menuVisible = command === "show" ? true : false;
        menu.style.display = menuVisible ? "block" : "none";
    };

    $(document).on('contextmenu', '.custom-right-click', function (e) {
        e.preventDefault();
        const origin = {
            left: e.pageX,
            top: e.pageY
        };
        MessageIdContextMenu = ($(this).attr("id").replace('msg-list-item-', ''));
        setPosition(origin);
    });

    menu.addEventListener('contextmenu', function (e) { //prevent right click on menu
        e.preventDefault();
    });

    $(window).on("click scroll resize", function () {
        if (menuVisible) toggleMenu("hide");
    });
    menuTargetContainer.scroll(function () {
        if (menuVisible) toggleMenu("hide");
    });


    $("#InboxRecivedMsgBtn").click(function () {
        closeOpenedMsg();
        CancelAllFetch();
        clearSearchMsg();
        LoadingAnimationInbox('#msgList', true);
        ReceivedInbox(true);
    });

    $("#InboxSentMsgBtn").click(function () {
        closeOpenedMsg();
        CancelAllFetch();
        clearSearchMsg();
        LoadingAnimationInbox('#msgList', true);
        SentInbox(true);
    });

    $('#btnSearchMsg').click(() => {
        var val = $('#searchMsgInputTxt').val();

        if (val && val.trim() !== '') {
            searchMsg(val);
        }

    });

    $("#searchMsgInputTxt").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#btnSearchMsg").click();
        }
    });

    $('#clearSearchMsgBtn').click(() => {
        closeOpenedMsg();
        CancelAllFetch();
        clearSearchMsg();
        LoadingAnimationInbox('#msgList', true);
        if (currentView == 1) {
            ReceivedInbox(true);
        } else if (currentView == 2) {
            SentInbox(true);
        }
    });

    $('#deleteMessageConfirmationBtn').click(() => {
        deleteMsg();
    });

    $(document).on('click', '#btnDeleteFromDetailsMsg', () => {
        IdOfMessageToDelete = $('#IdDetailsMsg').val(); 
        console.log('Id: ' + IdOfMessageToDelete);
    });
    
    $(document).on('click', '#btnDeleteMsgFromContextMenu', () => {
        IdOfMessageToDelete = MessageIdContextMenu; 
        console.log('IdC: ' + IdOfMessageToDelete);
    });

    $('#msgList').scroll(() => {
        let el = $('#msgList');

        if (el.scrollTop() + el.innerHeight() >= el[0].scrollHeight - 20) {

            if (hasNext === 'True' & !bottomMessageListHasBeenReached) {

                bottomMessageListHasBeenReached = true;

                console.log('end reached' + currentView);

                LoadingAnimationInbox('#msgList', false);

                if (currentView == 1) {
                    ReceivedInbox(false, Number(currentPage) + 1);
                } else if (currentView == 2) {
                    SentInbox(false, Number(currentPage) + 1);
                }

            }
        }
    });

    $(document).on('click', '.msg-list-item', function (e) {
        e.preventDefault();
        let id = $(this).attr("id").replace('msg-list-item-', '');
        CancelAllFetch();
        OpenMsg(id);
    });

    $(document).on('click', '#close-opened-msg', function () {
        closeOpenedMsg();
    });

    //Change icon color to white on hover
    $(document).on('mouseenter', '#close-opened-msg', function () {
        $("#img-close-opened-msg").addClass('white-filter');
    });
    //Remove white color to icon
    $(document).on('mouseleave', '#close-opened-msg', function () {
        $("#img-close-opened-msg").removeClass('white-filter');
    });

});
