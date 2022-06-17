  function call_onload() {
   $().AbortAjaxRequest(document);
  }
  jQuery(window).bind('beforeunload', function (e) {
   $().AbortAjaxRequest(document);
   return;
  });

  $(document).on({
   ajaxStart: function () { $("body").addClass("loading"); },
   ajaxStop: function () { $("body").removeClass("loading"); },
  });

  (function ($) {
   $.fn.AbortAjaxRequest = function (pDocument) {

    var xhrPool = [];
    $(pDocument).ajaxSend(function (e, jqXHR, options) {
     xhrPool.push(jqXHR);
    });

    $(pDocument).ajaxComplete(function (e, jqXHR, options) {
     xhrPool = $.grep(xhrPool, function (x) { return x != jqXHR });
    });

    var abort = function () {
     $.each(xhrPool, function (idx, jqXHR) {
      jqXHR.abort();
     });
    };

    var oldbeforeunload = window.onbeforeunload;
    window.onbeforeunload = function () {
     var r = oldbeforeunload ? oldbeforeunload() : undefined;

     if (r == undefined) {
      // only cancel requests if there is no prompt to stay on the page
      // if there is a prompt, it will likely give the requests enough time to finish    
      abort();
     }

     return r;
    }
   };
  })(jQuery);