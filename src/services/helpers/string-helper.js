export const getFileUrlFromSignedLinkAWS = (signedUrl) => {
  try {
    const { pathname } = new URL(signedUrl);
    const parts = pathname.split("/");
    let fileName = parts.pop() || parts.pop();
    return decodeURIComponent(fileName);
  } catch (err) {
    console.error("Invalid URL:", err);
    return null;
  }
};
