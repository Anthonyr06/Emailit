$(document).ready(() => {

    const maxSizeMB = '20';
    const maxFiles = '15';

    //$('#infoIconAdjuntarMensaje').attr('data-original-title', 'Máximo ' + maxSizeMB + 'MB por archivo');
    //$('#TextAdjuntarMensajeMaximaCant').html('M&aacute;ximo ' + maxFiles + ' archivos');
    
    $('#summernote').summernote({
        lang: 'en-US',
        disableResizeEditor: true,
        dialogsInBody: true,
        dialogsFade: true,
        tabDisable: true,
        placeholder: 'What\'s up? Write here:)(Required)',
        tabsize: 2,
        height: 250,
        callbacks: {
            onInit: function () {
                //adding customScroll to summernote
                $('div.note-editable.card-block').addClass("customScroll1");

                //Removing option to add image links
                $('.note-group-image-url').remove();
                $('.note-image-btn').remove();

                //Removing links in the footer
                //$('.note-modal > .modal-dialog > .modal-content > .modal-footer > p ').remove();

                //Removing alert message after one click
                $(".note-editable").on('click', function (e) {
                    $('.note-status-output').html('');
                });
            }
            //,
            //onImageLinkInsert: function (url) {

            //    getBase64FomFile(url, function (base64) {
            //        var imgNode = document.createElement('img');
            //        $(imgNode).attr('src', base64).css({ width: "50%" });
            //        $('#summernote').summernote('insertNode', imgNode);
            //    });

            //}
            ,
            onChange: function (contents, $editable) {
              //  console.log('onChange:', contents, $editable);

                let imagesSummernote = $(contents).find('img');

                if (imagesSummernote.length > maxFiles) {
                    $('.note-status-output').html(
                        '<div class="alert alert-danger">' +
                        'Can\'t add more than' + maxFiles + ' images.' +
                        '</div>'
                    );
                    //imagesSummernote.slice(Math.max(imagesSummernote.length - maxFiles, 0));
                    let extraElements = imagesSummernote.slice(maxFiles, imagesSummernote.length);

                    for (var i = 0; i < extraElements.length; i++) {
                        contents = contents.replace(extraElements[i].outerHTML, '');
                    }
                    $('#summernote').summernote('code', contents);

                }
            },
            onImageUpload: function (files) {

                for (var i = 0; i < files.length; i++) {

                    let file = files[i];

                    if (!file.type.match('image')) {
                        continue;
					}

                    let sizeMB = file.size / 1024 / 1024;

                    if (sizeMB > maxSizeMB) {
                        $('.note-status-output').html(
                            '<div class="alert alert-danger">' +
                            'The size of the images should not exceed ' + maxSizeMB + 'MB.' +
                            '</div>'
                        );
                        continue;
                    }

                    getBase64FomFile(file, function (base64) {

                        let imagesSummernote = $(".note-editable").find('img');

                        if (imagesSummernote.length > maxFiles) {
                            $('.note-status-output').html(
                                '<div class="alert alert-danger">' +
                                'Can\'t add more than' + maxFiles + ' images.' +
                                '</div>'
                            );
                            return;
                        }

                        let imgNode = document.createElement('img');

                        $(imgNode).attr({'src': base64, 'data-filename': file.name })
                            .css({ width: "25%", 'max-width': "100%" });

                        $('#summernote').summernote('insertNode', imgNode);

                    });

                }
            }
            //,onMediaDelete: function (target) {
            //    alert(target[0].src);

            //    if (target.is('img')) {

            //        console.log(target);
            //    }

            //    $(target[0]).remove();
            //}
        },
        toolbar: [
            ['style', ['style', 'clear']],
            ['font', ['bold', 'underline', 'strikethrough', 'superscript', 'subscript']],
            ['fontname', ['fontname']],
            ['fontsize', ['fontsize']],
            ['color', ['forecolor']], 
            ['backcolor', ['backcolor']],
            ['para', ['ul', 'ol', 'paragraph', 'height']],
            ['table', ['table','hr']],
            ['insert', ['link', 'picture']],
            ['view', ['fullscreen', 'help']],
            ['misc', ['undo', 'redo']],
        ],
    });

    let magicSuggestParams = {
        style: 'outline: none !important; box-shadow: none!important; border: 1px solid #DEE2E6!important; -webkit-box-shadow: none!important;',
        data: '/Message/Create?',
        method: 'get',
        contentType: "application/json",
        dataType: "json",
        dataUrlParams: { handler: 'Emails' },
        //beforeSend: (xhr) => {
        //    xhr.setRequestHeader("RequestVerificationToken",
        //        $('input:hidden[name="__RequestVerificationToken"]').val());
        //},
        queryParam: 'search',
        vtype: 'email',
        displayField: 'email',
        selectionCls: 'rounded-pill',
        hideTrigger: true,
        renderer: (data) => {
            let fullName = data.name + ' ' + data.lastname;
            return '<div class="font-weight-bold">' + fullName + '</div><small class="text-black-50">' + data.email + '</small>'
        },
        valueField: 'userId',
        highlight: false,
        placeholder: 'Write an email (Required)',
        // noSuggestionText: 'Sin sugerencias {{query}}',
        noSuggestionText: 'No suggestions.',
        maxSelection: null,
        required: true,
        selectionRenderer: (data) => {
            return '<img style="width:.9rem;" class="mb-1 mr-1" src="/icon/user-2.svg"/>' + data.email;
        },
        minChars: 3,
        minCharsRenderer: (v) => {
            return 'Please write ' + v + ' more letter' + (v > 1 ? 's' : '');
        },

    }

    let ms1 = $('#recipientsTxtInput').magicSuggest(magicSuggestParams);
magicSuggestParams.placeholder = ' Write an email';
    let ms2 = $('#ccRecipientsTxtInput').magicSuggest(magicSuggestParams );

    $('.ms-ctn input').addClass('border-0').attr('autocomplete','ChillOutChrome');

    $('#confidentialCheck').change(() => {
        if ($('#confidentialCheck').is(":checked")) {
            $('#ConfidencialChkImg').removeClass('textmutedbootstrap-filter');
        } else {
            $('#ConfidencialChkImg').addClass('textmutedbootstrap-filter');
        }
    });

    $(document).on('click', '#sendMsg', () => {

        if (invalid()) {
            $('#newMessageModalTxt').html("Fields marked as <strong>(Required)</strong> cannot be empty.");
            $('#newMessageModal').show();
        } else {
            sendMsg();
		}
    });

    function invalid() {

        let error = false;

        let v = ms1.getValue().toString();

        if (!v || v.trim() === '') {
            error = true;
        }

        v = $('#MessageTittleTxtInput').val();

        if (!v || v.trim() === '') {
            error = true;
        }

        if ($('#summernote').summernote('isEmpty')) {
            error = true;
        }

        return error;
    };

    let currentFilesToUpload = [];

    function ClearNewMessageModalData() {
        ms1.clear();
        ms2.clear();
        $('#MessageTittleTxtInput').val('');
        $('#summernote').summernote('reset');
        $('#confidentialCheck').prop("checked", false).change();
        $('#prioritySelectList').val(0);
        RemoveAllAttachedFiles();
	}

    $(document).on('click', '#newMessageBtn', () => {
        ClearNewMessageModalData();
    });

    function sendMsg() {
        let appMessage, idMenssage;

        $('#newMessageLoadingLogo').show();

        var data = new FormData();

        //let test = 0;
        //currentFilesToUpload.forEach(function (file, i) {
        //    test = test + file.size;
        //});

        data.append('recipientsId', ms1.getValue());
        data.append('ccRecipientsId', ms2.getValue());
        data.append('tittle', $('#MessageTittleTxtInput').val());
        data.append('body', $('#summernote').summernote('code'));
        data.append('Confidential', $('#confidentialCheck').is(":checked") );
        data.append('priority', $('#prioritySelectList').val() );
        currentFilesToUpload.forEach(function (file, i) {
            data.append('formFiles', file);
        });
        //console.log('tSize: ' + test);
        fetch('/Message/Create', {
            method: 'POST',
            body: data,
            headers: {
                RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val(),
            },
            redirect: "manual"
        })
            .then((res) => {
                appMessage = res.headers.get('app-message'); 
             //   idMenssage = res.headers.get('id-message');
                if (!res.ok) throw new Error(res.error);
                return res.text();
            })
            .then(() => {

                if (appMessage) {
                    $('#newMessageModalTxt').html(appMessage);
                    $('#newMessageModal').show(); 
                } else {
                    ClearNewMessageModalData();

                    $('#NuevoOficioModal').modal('hide');
                    closeOpenedMsg();
                    clearSearchMsg();
                    LoadingAnimationInbox('#msgList', true);
                    SentInbox(true);
                   // OpenMsg(idMenssage);
				}
            })
            .catch(error => {
                if (error.code !== DOMException.ABORT_ERR) {
                    console.log(error);

                    //TO DO: Instead of telling the user there was an error, reopen the modal and unlock the "newmessage" button
                    let Error = '<div class="mt-5 p-3 text-center">' +
                        '<div class="bg-white mx-auto w-75 rounded-pill p-3">' +
                        '<h5 class="pl-2 pt-2">' +
                        '<img src="/icon/error.svg" class="mr-2 mb-1" style="width:3rem;" />Ha ocurrido un error.</h5>' +
                        '<p class="text-muted">Actualiza la pagina.</P>' +
                        '</div>';
                    $('#NuevoOficioModal > div').removeClass('modal-xl');
                    $('#NuevoOficioModal .modal-body').html(Error);
                }
            })
            .finally(function () {

                $('#newMessageLoadingLogo').hide();

            });
    };

    $(document).on('click', '#newMessageModal > button.close', () => { $('#newMessageModal').hide(); });

    $("#file-upload-msg").change((evt) => {

        let tgt = evt.target || window.event.srcElement;
        let files = tgt.files;

        for (var i = 0; i < files.length; i++) {

            let file = files[i];

            let sizeMB = file.size / 1024 / 1024;

            if (currentFilesToUpload.indexOf(file) > -1) {
                continue;
            }

            if (sizeMB > maxSizeMB) {
                alert('The file ' + file.name + ' is too large');
                continue;
            }

            if (file.type.match('image')) {

                getBase64FomFile(file, function (base64) {
                    NewAttachedFile(file, base64, maxFiles);
                });
            }
            else {

                let data = getMimeTypeIconUrl(file.type);

                NewAttachedFile(file, data, maxFiles);

            }

        }
        //The inputfile is emptied to be able to re-add a file that had been previously deleted
        $("#file-upload-msg").val('');
    });

    $(document).on('click', '.close-doc-file-upload-msg-box', function (e) {

        let container = $(this).parent().parent(),
            fileName = $(this).siblings('img').attr('title');

        let itemId = ('#' + container.attr('id'));
        console.log(itemId);

        removeAttachedFile(container, fileName);

    });

    function removeAttachedFile(container, fileName) {

        for (var i = 0; i < currentFilesToUpload.length; i++) {
            if (currentFilesToUpload[i].name == fileName) {

                container.toggleClass('animate__bounceIn animate__bounceOut');

                currentFilesToUpload.splice(i, 1);

                container.on('animationend', () => {
                    container.remove();
                    activeFileUploadMsgButton();
                    updateNewMessageAttachmentsBadge();
                });

                break;
            }
        }
    }

    function RemoveAllAttachedFiles() {
        currentFilesToUpload = [];
        $("#file-upload-msg").val('');
        $("#file-upload-msg-box").children(":not(p)").remove();
        updateNewMessageAttachmentsBadge();
        activeFileUploadMsgButton();
    }

    function NewAttachedFile(file, src, maxFiles = 15) {

        if (currentFilesToUpload.length < maxFiles) {
            currentFilesToUpload.push(file);
            $('#file-upload-msg-box').append(htmlFileAddedPreview(src, file));

        }
        if (currentFilesToUpload.length >= maxFiles) {
            activeFileUploadMsgButton(false);
        }

        updateNewMessageAttachmentsBadge();
    }

    function updateNewMessageAttachmentsBadge() {
        if (currentFilesToUpload.length > 0) {
            $('#file-upload-msg-box').children('p').hide();
            $('#newMessageAttachmentsBadge').html(currentFilesToUpload.length);
            $('#newMessageAttachmentsBadge').show();
        } else {
            $('#file-upload-msg-box').children('p').show();
            $('#newMessageAttachmentsBadge').hide();
        }
    }


    function activeFileUploadMsgButton(active = true) {
        let input = $("#file-upload-msg"),
            btn = $('label[for="file-upload-msg"]');

        input.prop('disabled', !active);
        btn.toggleClass('bg-success', active).toggleClass('bg-secondary', !active);
        btn.css('cursor', active ? 'pointer' : 'not-allowed');
    }

    function htmlFileAddedPreview(src, file) {

        return '<div id="file-added-' + file.name + '" class="col-3 mt-2 animate__animated animate__bounceIn"> ' +
            '<span class="w-25 " style=" position: relative;">' +
            '<img src="' + src + '" class="w-100" title="' + file.name + '" >' +
            '<span class="text-white rounded-circle pb-1 px-2 close-doc-file-upload-msg-box" ' +
            'style="display: inline;cursor:pointer; position: absolute; right: -10px; background-color: red;">&times;</span></span></div>';
    }

    $(document).on('click', '#btnForwardFromDetailsMsg', () => {
        forward($('#IdDetailsMsg').val());
    });

    $(document).on('click', '#btnForwardFromContextMenu', () => {
        forward(MessageIdContextMenu);
    });

    function forward(id) {
        console.log('IdToForward: ' + id);

        $('#newMessageLoadingLogo').show();

        fetch('/Message/Details?' + new URLSearchParams({
            id: id,
            handler: 'Json'
        }), { redirect: "manual"})
            .then((res) => {
                if (!res.ok) throw new Error(res.error);
                return res.json();
            })
            .then(data => {
                ms1.clear();
                ms2.clear();
                RemoveAllAttachedFiles();

                $('#MessageTittleTxtInput').val(data.tittle);
                $('#confidentialCheck').prop("checked", data.confidential).change();
                $('#prioritySelectList').val(data.priority);


                //Getting src from images of the message body 
                let domParser = new DOMParser();
                let docElement = domParser.parseFromString(data.body, "text/html").documentElement;
                let summernoteImgs = docElement.getElementsByTagName("img");

               // $('#summernote').summernote('code', docElement);

                for (var i = 0; i < data.attachments.length; i++) {
                    $('#file-upload-msg-box').append('<span>&nbsp;Loading...&nbsp;</span>');
                }

                for (var i = 0; i < data.attachments.length; i++) {

                    let inf = data.attachments[i];

                    fetch(inf.url)
                        .then((res) => {
                            if (!res.ok) throw new Error(res.error);
                            return res.blob();
                        })
                        .then(blobFile => {
                            let file = new File([blobFile], inf.nombre, { type: blobFile.type });

                            let sizeMB = file.size / 1024 / 1024;

                            if (sizeMB > maxSizeMB) {
                                alert('The file ' + file.name + ' is too large');
                            }
                            else if (file.type.match('image')) {

                                var isSummernoteImg = false; //If the image is attached in the body of the message

                                getBase64FomFile(file, function (base64) {
                                    for (var i = 0; i < summernoteImgs.length; i++) {
                                        if (summernoteImgs[i].getAttribute("src").indexOf(inf.url) > -1) {
                                            isSummernoteImg = true;
                                            summernoteImgs[i].src = base64;
                                            summernoteImgs[i].setAttribute("data-filename", inf.nombre);
                                        }
                                    }
                                    if (!isSummernoteImg) {
                                        NewAttachedFile(file, base64, maxFiles);
									}
                                });
                            }
                            else {
                                let data = getMimeTypeIconUrl(file.type);
                                NewAttachedFile(file, data, maxFiles);
                            }

                        })
                        .finally(function () {
                            //    $('#summernote').summernote('code', docElement);
                            $('#file-upload-msg-box > span:last').remove();
                        });
                }

                $('#summernote').summernote('code', docElement);
            })
            .catch((error) => {
                console.log(error);
            })
            .finally(function () {
                $('#newMessageLoadingLogo').hide();
            });

    }

});
