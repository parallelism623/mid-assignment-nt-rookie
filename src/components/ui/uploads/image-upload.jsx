import React, { useEffect, useState } from "react";
import { Upload, message } from "antd";

import { PlusOutlined, CloseOutlined } from "@ant-design/icons";

const ImageUpload = ({ setImage, defaultUrl = null }) => {
  const [url, setUrl] = useState(null);
  useEffect(() => {
    setUrl(defaultUrl);
  }, [defaultUrl]);
  const beforeUpload = (file) => {
    setImage(file);
    const purl = URL.createObjectURL(file);
    setUrl(purl);

    return false;
  };
  const handleOnClickDeleteImage = (e) => {
    e.stopPropagation();
    setImage(null);

    if (url) {
      URL.revokeObjectURL(url);
      setUrl(null);
    }
  };
  return (
    <>
      <Upload
        maxCount={1}
        listType="picture-card"
        className="avatar-uploader w-fit !overflow-hidden !relative"
        showUploadList={false}
        beforeUpload={beforeUpload}
      >
        <CloseOutlined
          onClick={(e) => {
            handleOnClickDeleteImage(e);
          }}
          style={{
            position: "absolute",
            top: 4,
            right: 4,
            fontSize: 16,
            color: "rgba(0,0,0,0.45)",
            cursor: "pointer",
            background: "rgba(255,255,255,0.8)",
            borderRadius: "50%",
            padding: 2,
          }}
        />
        {url ? (
          <img src={url} alt="avatar" style={{ width: "100%" }}></img>
        ) : (
          <div>
            <PlusOutlined />
            <div style={{ marginTop: 8 }}>Upload</div>
          </div>
        )}
      </Upload>
    </>
  );
};

export default ImageUpload;
