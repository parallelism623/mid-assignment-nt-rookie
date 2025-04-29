export const validationData = {
  categoryDescriptionMaxLength: 2000,
  categoryNameMaxLength: 100,
  bookTitleMaxLength: 100,
  bookAuthorMaxLength: 100,
  bookDescriptionMaxLength: 2000,
  bookBorrowingDetailNotedMaxLength: 2000,
  userFirstNameMaxLength: 100,
  userLastNameMaxLength: 100,
  userUserNameMaxLength: 32,
  userPasswordMaxLength: 32,
  userPasswordMinLength: 8,
  userEmailMaxLength: 100,
  userPhoneNumberMaxLength: 20,
  userPasswordRegexPattern:
    /^(?=.{8,32}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/,
  bookBorrowedExtendDueDate: 1,
};
