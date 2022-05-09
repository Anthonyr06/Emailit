
$(document).ready(function () {

        //Automatic size of the textarea.
    $("textarea").on('change keyup click keydown paste cut', function () {
        $(this).height(0).height(this.scrollHeight);
    });
    $("textarea").change();

        //Prevent the dropdown from closing when an object inside the dropdown is clicked.
    $('.dropdown-no-close').on("click.bs.dropdown", function (e) {
        e.stopPropagation();
    });


        //Add front-end validation to checkbox lists [Condition: at least one checkbox must be selected]
    $("form").submit(function (e) {
        if ($(".check-list-at-least-one").length && $('.check-list-at-least-one:checked').length < 1) {
            e.preventDefault();

            $(".check-list-at-least-one-msg").removeAttr('hidden');

            return false;
        }
    });
    $('.check-list-at-least-one').click(function () {
        if ($('.check-list-at-least-one:checked').length > 0) {

            $(".check-list-at-least-one-msg").attr('hidden', true);
        } else {
            $(".check-list-at-least-one-msg").removeAttr('hidden');

        }
    });


        //Make a scroll to the position of the page that the user was, before reloading.
    window.onload = function () {
        var pos = window.name || 0;
        window.scrollTo({ left: 0, top: pos, behavior: 'smooth' });
    }
    window.onunload = function () {
        window.name = self.pageYOffset || (document.documentElement.scrollTop + document.body.scrollTop);
    }

    //Enable bootstrap tooltip
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });

    //Used to prevent a modal's scroll from stopping working when another modal is opened in front of it
    $(document).on('hidden.bs.modal', '.modal', () => {
        $('.modal:visible').length && $(document.body).addClass('modal-open');
    });

});

//Class to use the Animate.css library
const animateCSS = (element, animation, prefix = 'animate__') =>
    // We create a Promise and return it
    new Promise((resolve, reject) => {
        const animationName = `${prefix}${animation}`;
        const node = document.querySelector(element);

        node.classList.add(`${prefix}animated`, animationName);

        // When the animation ends, we clean the classes and resolve the Promise
        function handleAnimationEnd() {
            node.classList.remove(`${prefix}animated`, animationName);
            resolve('Animation ended');
        }

        node.addEventListener('animationend', handleAnimationEnd, { once: true });
    });

function getMimeTypeIconUrl(fileType) {

    if (fileType.match('application/pdf')) {
        return 'icon/pdf.svg';
    }
    else if (fileType.match('application/msword') || fileType.match('wordprocessingml')) {
        return 'icon/doc.svg';
    }
    else if (fileType.match('application/vnd.ms-excel') || fileType.match('spreadsheetml')) {
        return 'icon/xls.svg';
    }
    else {
        return 'icon/document.svg';
    }
}

function getBase64FomFile(img, callback) {
    let fileReader = new FileReader();
    fileReader.addEventListener('load', function (evt) {
        callback(fileReader.result);
    });
    fileReader.addEventListener('error', (e) => {
        console.log(Reader.error);
    });
    fileReader.readAsDataURL(img);
}
