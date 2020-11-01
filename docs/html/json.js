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
  var entities = [];

  $.each($(".entity-wrapper"), function(index, entityWrapper) {
    var entity = new Object();
    entity.Name = $(entityWrapper).find(".entity-name").val();
    entity.Table = $(entityWrapper).find(".entity-table").val();
    entity.Properties = [];
    
    $.each($(entityWrapper).find(".property-wrapper"), function(index, propertyWrapper) {
      var property = new Object();

      property.Name = $(propertyWrapper).find(".property-name").val();
      property.Column = $(propertyWrapper).find(".property-column").val();
      property.Primitive = $(propertyWrapper).find(".property-primitive").val();
      property.IsPrimaryKey = $(propertyWrapper).find(".property-primarykey").val() === "true";
      property.IsUnique = $(propertyWrapper).find(".property-nullable").val() === "true";
      property.IsNullable = $(propertyWrapper).find(".property-unique").val() === "true";
      property.Validations = [];

      $.each($(propertyWrapper).find(".validation-wrapper"), function(index, validationWrapper) {
        var validation = new Object();

        validation.Type = $(validationWrapper).find(".validation-type").val();
        validation.Value = $(validationWrapper).find(".validation-value").val();
        validation.Depends = new Object();
        validation.Depends.On = $(validationWrapper).find(".validation-depends-on").val();
        validation.Depends.When = $(validationWrapper).find(".validation-depends-when").val();
        
        if (validation.Type !== "" ||
            validation.Value !== "" ||
            validation.Depends.On !== "" ||
            validation.Depends.When !== "") {
          property.Validations.push(validation);
        }
      });

      entity.Properties.push(property);
    });

    entities.push(entity);
  });

  var project = new Object();
  project.Name = $(".project-name-input").val();
  project.Database = $(".project-database").val();
  project.Entities = entities;

  var json = JSON.stringify(project, null, 2);

  $(".jsonarea").text(json);
}