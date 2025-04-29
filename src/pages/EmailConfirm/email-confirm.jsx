import React, { useState, useEffect } from "react";
import { Card, Input, Button, Spin } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import { useLocalStorage } from "../../components/hooks/useStorage";
import { authenServices } from "../../services/authenServices";
import { useNavigate } from "react-router";

const EmailConfirm = () => {
  const [code, setCode] = useState("");
  const [loading, setLoading] = useState(false);
  const [counter, setCounter] = useState(60);
  const navigate = useNavigate();
  const [userEmailConfirm, , removeUserEmailConfirm] = useLocalStorage(
    "user-email-confirm",
    null
  );

  // Countdown effect
  useEffect(() => {
    if (counter > 0) {
      const timer = setTimeout(() => setCounter(counter - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [counter]);

  const handleChange = (e) => {
    const val = e.target.value.replace(/\D/g, "").slice(0, 6);
    setCode(val);
  };

  const handleSubmit = () => {
    if (code.length === 6) {
      setLoading(true);
      authenServices
        .emailConfirm({ code, username: userEmailConfirm.username })
        .then(() => {
          setCode("");
          removeUserEmailConfirm();
          navigate("/signin");
        })
        .finally(() => setLoading(false));
    }
  };

  const handleResend = () => {
    if (counter === 0) {
      authenServices.emailConfirmRefresh({
        username: userEmailConfirm.username,
      });
      setCounter(60);
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-blue-50 p-4">
      <h1 className="text-4xl font-bold text-center mb-1">Book Management</h1>
      <p className="text-center text-gray-600 mb-6">
        Confirm your email to activate your account
      </p>
      <Card
        title={
          <div className="w-full text-center">
            <span className="text-2xl font-bold">Confirm Email</span>
          </div>
        }
        className="w-100 p-4 rounded-2xl shadow-md"
        headStyle={{ borderBottom: "none", paddingBottom: 0 }}
        bodyStyle={{ paddingTop: 0 }}
      >
        <p className="text-gray-600 mb-4 text-center">
          Please enter the 6-digit code we sent to your email.
        </p>

        {!loading ? (
          <Input
            value={code}
            onChange={handleChange}
            onPressEnter={handleSubmit}
            maxLength={6}
            placeholder="Enter code"
            className="text-center text-xl tracking-widest rounded-lg !mb-5"
          />
        ) : (
          <Spin
            className="w-full flex justify-center !mb-5"
            indicator={
              <LoadingOutlined
                style={{
                  fontSize: 24,
                  color: "var(--background-color-active-dark-mode)",
                }}
                spin
              />
            }
          />
        )}

        <div className="flex gap-2">
          <Button
            type="primary"
            onClick={handleSubmit}
            disabled={code.length !== 6 || loading}
            className="flex-1 rounded-lg"
          >
            Submit
          </Button>
          <Button
            onClick={handleResend}
            disabled={counter > 0 || loading}
            className="flex-1 rounded-lg"
          >
            {counter > 0 ? `Send token (${counter}s)` : "Send token"}
          </Button>
        </div>
      </Card>
    </div>
  );
};

export default EmailConfirm;
