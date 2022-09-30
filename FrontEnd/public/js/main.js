
var moveContent = () =>
{
    $('body').css('padding-top', parseInt($('#nav').css("height")) + 20);
}



$(document).ready(() =>
{
    var scrollSpy = new bootstrap.ScrollSpy(document.body, {
        target: '#nav'
    })
    
    $('.n-link').on('click', () =>
    {
        //prevent further clicking here?
        setTimeout(() => 
        {
            $('.navbar-collapse').collapse('hide');
        }, 900);
    });


    

 

});
