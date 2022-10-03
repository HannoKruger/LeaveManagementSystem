'use strict'

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
            url: "/leave",
            type: 'GET',
            dataType: 'json', // added data type
            data: { FirstName: $("#form-name").val(), LastName: $("#form-surname").val() },
            success: function (res)
            {
                console.log("c# resp: "+res);             
                $("#form-current-leave").val(res);
            }
        });

        //let leave = $("#form-current-leave");
    }
}
function CalculateLeave()
{
    if($("#form-leave-start").val() != "" && $("#form-leave-end").val() != "")
    {
        console.log("CalculateLeave");

        let start = new Date($("#form-leave-start").val());
        let end = new Date($("#form-leave-end").val());

        let days = (end - start) / (1000 * 60 * 60 * 24);

        
        $("#form-days-taken").val(days);

        if($("#form-current-leave").val() != "") {

            console.log("CurrentLeave " + $("#form-current-leave").val());
            console.log("DaysTaken: " + $("#form-days-taken").val());
            console.log("DaysLeft: " + (parseInt($("#form-current-leave").val()) - days));

            //$("form-days-left").val(parseInt($("#form-current-leave").val()) - days);
            document.getElementById("form-days-left").value = parseInt($("#form-current-leave").val()) - days;
        }
    }

}


$(document).ready(() =>
{
    var frm = $('#form');


    $(frm).submit(function (e)
    {

        let valid = true;

        e.preventDefault(); // avoid to execute the actual submit of the form.


        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.querySelectorAll('.needs-validation')

        Array.prototype.slice.call(forms).forEach(function (form)
        {
            if (!form.checkValidity())
            {
                //console.log("Form not valid");
                valid = false;
                return;
            }
        });
        if (!valid) return;



        //var paramsToSend = {};

        //paramsToSend['client'] = $('form[name="client"]').serialize();

        //console.log("params to send: "+JSON.stringify(paramsToSend));


        $.ajax(
            {
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


});