$(document).ready(function() {
  watcherInputAndSelectBlur();
});

function watcherInputAndSelectBlur() {
  $(document).on("blur", ".bootstrap-wrapper input", function(element) {
    console.log("blur")
  });
}