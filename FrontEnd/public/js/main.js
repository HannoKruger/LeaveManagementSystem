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
            dataType: 'json',
            data: { FirstName: $("#form-name").val(), LastName: $("#form-surname").val() },
            success: function (res)
            {
                res = JSON.parse(res);

                //console.log("c# resp: "+res);           
                if(res === "EmployeeNotFound")
                {
                    alert("Employee not found");
                }
                
                $("#form-current-leave").val(res);
            }
        });     
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
                valid = false;
                return;
            }
        });
        if (!valid) return;



        $.ajax(
            {
                type: frm.attr('method'),
                url: frm.attr('action'),
                data: frm.serialize(),
                success: function (data)
                {
                    //console.log('Submission was successful.');                 
                },
                error: function (data)
                {
                    console.log('An error occurred.');
                    console.log(data);
                }
            });       
    });


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