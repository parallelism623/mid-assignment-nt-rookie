import axiosClient from "../config/axiosConfig";

export const bookReviewServices = {
  create: (requestModel) => {
    return axiosClient.post("/book-reviews", requestModel);
  },
};
