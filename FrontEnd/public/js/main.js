
var moveContent = () =>
{
    $('body').css('padding-top', parseInt($('#nav').css("height")) + 20);
}



function GetCurrentLeave() 
{
    if ($("#form-name").val() != "" && $("#form-surname").val() != "")
    {

        console.log("GetCurrentLeave");

        $.ajax({
            url: "/get-leave",
            type: 'GET',
            dataType: 'json', // added data type
            success: function(res) {
                //console.log(JSON.stringify(res));
                //console.log("Leave: " + res);
                $("#form-current-leave").val(res);           
            }
        });


        //let leave = $("#form-current-leave");


        
    }


}



$(document).ready(() =>
{

    //#form
    //form-post


    //     // this is the id of the form


    var frm = $('#form');

    $(frm).submit(function (e)
    {

        e.preventDefault(); // avoid to execute the actual submit of the form.

        //var paramsToSend = {};

        //paramsToSend['client'] = $('form[name="client"]').serialize();

        //console.log("params to send: "+JSON.stringify(paramsToSend));


        $.ajax({
            type: frm.attr('method'),
            url: frm.attr('action'),
            data: frm.serialize(),
            success: function (data)
            {
                //console.log('Submission was successful.');
                //console.log(data);
            },
            error: function (data)
            {
                console.log('An error occurred.');
                console.log(data);
            }
        });



    });


    // $("#form").ajaxSubmit({ url: '/form-post', type: 'post' });










    (function ()
    {
        'use strict'

        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.querySelectorAll('.needs-validation')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form)
            {
                form.addEventListener('submit', function (event)
                {
                    if (!form.checkValidity())
                    {
                        event.preventDefault()
                        event.stopPropagation()
                    }

                    form.classList.add('was-validated')
                }, false)
            })
    })()





    // var scrollSpy = new bootstrap.ScrollSpy(document.body, {
    //     target: '#nav'
    // })


    // $('.n-link').on('click', () =>
    // {
    //     //prevent further clicking here?
    //     setTimeout(() => 
    //     {
    //         $('.navbar-collapse').collapse('hide');
    //     }, 900);
    // });



});
