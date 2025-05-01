import axiosClient from "../config/axiosConfig";

export const imageStorageServices = {
  upload: (formData) =>
    axiosClient.post("/images/", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    }),
  delete: (requestModel) =>
    axiosClient.delete("/images", {
      headers: { "Content-Type": "application/json" },
      data: requestModel,
    }),
};
