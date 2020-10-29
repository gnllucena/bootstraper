$(document).ready(function() {
  watcherInputAndSelectBlur();
});

function watcherInputAndSelectBlur() {
  $(document).on("blur", ".content input", function(element) {
    toJson();
  });

  $(document).on("blur", ".content select", function(element) {
    toJson();
  });
}

function toJson() {
  $(".content")

  var project = new Object();
  project.Name = $(".project-name").val();
  project.Database = $(".project-database").val();
  project.Entities = [
    
  ];

  var json = JSON.stringify(project);

  $(".jsonarea").text(json);
}