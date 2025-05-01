import axiosClient from "../config/axiosConfig";
import qs from "qs";
export const bookReviewServices = {
  create: (requestModel) => {
    return axiosClient.post("/book-reviews", requestModel);
  },
  get: (queryParameters) => {
    return axiosClient.get("/book-reviews", {
      params: queryParameters,
      paramsSerializer: (params) => {
        return qs.stringify(params);
      },
    });
  },
};
