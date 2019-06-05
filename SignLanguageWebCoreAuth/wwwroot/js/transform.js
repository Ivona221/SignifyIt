$(document).ready(function () {
	var slideIndex = 0;
	showSlides();

    $("#transform").click(function () {
        var model = {
            Text: $("#insert-text").val(),
            Images: null
        };
        Notifications().showLoading();
        $.ajax({
            url: "/Images/Transform",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
	        console.log(res.images);
	        $(".slideshow-container").html("");
            //var redirectUrl = res.redirectUrl;
            //window.location.href = redirectUrl;
            if (res.images != null) {
                if (res.images.length == 1) {
                    image = res.images[0].image;
                    $(".slideshow-container").append(
                        '<img src="' + image + '" style="width:100%"><div class="image-meaning">'+ res.images[0].meaning +'</div>'
                    );
                    Notifications().hideLoading();
                } else {
                    for (var image in res.images) {
                        var imageName = res.images[image].image;

                        $(".slideshow-container").append(
                            '<div class="mySlides"> <img src="' + imageName + '" style="width:100%"> <div class="image-meaning">'+ res.images[image].meaning +'</div></div>'
                        );
                    }
                    Notifications().hideLoading();
                    showSlides();
                }
				         
                
			}
        });
    });

    function showSlides() {
	    var i;
	    var slides = document.getElementsByClassName("mySlides");
	    //var dots = document.getElementsByClassName("dot");
		if (slides == null || slides.length === 0) {
			return;
		}
	    for (i = 0; i < slides.length; i++) {
		    slides[i].style.display = "none";  
	    }
	    slideIndex++;
	    if (slideIndex > slides.length) {slideIndex = 1}    
//	    for (i = 0; i < dots.length; i++) {
//		    dots[i].className = dots[i].className.replace(" active", "");
//	    }
	    slides[slideIndex-1].style.display = "block";  
	    //dots[slideIndex-1].className += " active";
	    setTimeout(showSlides, 4000); // Change image every 2 seconds
    }
});