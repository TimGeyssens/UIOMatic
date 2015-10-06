(function($) {

  // Making elements equal height
    var equalheight = function(container){

      var currentTallest = 0,
        currentRowStart = 0,
        rowDivs = new Array(),
        $el,
        topPosition = 0;

      $(container).find('.equal').each(function() {

        $el = $(this);
        $($el).height('auto')
        topPostion = $el.position().top;

        if (currentRowStart != topPostion) {
          for (currentDiv = 0 ; currentDiv < rowDivs.length ; currentDiv++) {
              rowDivs[currentDiv].height(currentTallest);
        }
          rowDivs.length = 0; // empty the array
          currentRowStart = topPostion;
          currentTallest = $el.height();
          rowDivs.push($el);
        } else {
          rowDivs.push($el);
          currentTallest = (currentTallest < $el.height()) ? ($el.height()) : (currentTallest);
        }
        for (currentDiv = 0 ; currentDiv < rowDivs.length ; currentDiv++) {
          rowDivs[currentDiv].height(currentTallest);
        }
      });
    };

  // Check for window width before resizing
  function equalHeightChecker () {
    if ( window.innerWidth > 767 && !heightIsSet ) {
      $('.equalizer')
        .each(function(){
          equalheight(this);
          heightIsSet = true;
        });
    }
    else if (window.innerWidth<768 && heightIsSet) {
      $('.equalizer')
        .each(function(){
          $(this).find('.equal').each(function () {
            this.style.height = 'auto';
          });
          heightIsSet = false;
        });
    }
  }

  // Initialize equal height script
  var heightIsSet;

  // On load
  $(window).load(function() {
   equalHeightChecker();
  });

  // and on resize
  $(window).resize(function(){
    equalHeightChecker();
  });

  // Navigation
  $('#toggle').click(function(){
    $('.has-child').removeClass('selected');
    $('nav').toggleClass('open');
    $('.cross').toggleClass('open');
  });

  $('.has-child').click(function(){
    if ( window.innerWidth < 768 ) {
      if ( $( this ).hasClass('selected')){
        $('.has-child').removeClass('selected');    
      } else {
        $('.has-child').removeClass('selected'); 
        $(this).toggleClass('selected');
      }
    }
  });

})(jQuery);