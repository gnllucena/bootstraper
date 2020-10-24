function clearSelect(element) {
  $(element).empty();
  $(element).append($('<option></option>'));
}

function getValidations(primitive) {
  var validations = [];

  switch (primitive) {
    case "int":
      validations = PROPERTY_INT_VALIDATIONS;
      break;
    case "DateTime":
      validations = PROPERTY_DATETIME_VALIDATIONS;
      break;
    case "string":
      validations = PROPERTY_STRING_VALIDATIONS;
      break;
    case "decimal":
      validations = PROPERTY_DECIMAL_VALIDATIONS;
      break;
    case "bool":
      validations = PROPERTY_BOOL_VALIDATIONS;
      break;
    default:
      throw Error(`Primitive ${primitive} not implemented`);
  }

  return validations;
}

function getDependsWhen(primitive) {
  var dependsWhen = [];

  switch (primitive) {
    case "int":
      dependsWhen = PROPERTY_INT_DEPENDS_WHEN;
      break;
    case "DateTime":
      dependsWhen = PROPERTY_DATETIME_DEPENDS_WHEN;
      break;
    case "string":
      dependsWhen = PROPERTY_STRING_DEPENDS_WHEN;
      break;
    case "decimal":
      dependsWhen = PROPERTY_DECIMAL_DEPENDS_WHEN;
      break;
    case "bool":
      dependsWhen = PROPERTY_BOOL_DEPENDS_WHEN;
      break;
    default:
      throw Error(`Primitive ${primitive} not implemented`);
  }

  return dependsWhen;
}