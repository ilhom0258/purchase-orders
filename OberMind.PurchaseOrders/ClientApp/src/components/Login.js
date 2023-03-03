import React, { useState } from "react";
import "./Login.css";
import { Button, Form, Input, FormGroup, Label } from "reactstrap";

async function loginUser(credentials) {
  return fetch("api/users/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      accept: "text/plain",
    },
    body: JSON.stringify(credentials),
  }).then(async (data) => await data.json());
}

async function registerUser(userInfo) {
  return fetch("api/users/register", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      accept: "text/plain",
    },
    body: JSON.stringify(userInfo),
  }).then((data) => data);
}

export function Login({ setToken }) {
  const [isLogin, setIsLogin] = useState(true);
  const [login, setLogin] = useState();
  const [password, setPassword] = useState();
  const [name, setName] = useState();

  const onRegister = async (event) => {
    event.preventDefault();
    await registerUser({ login: login, password: password, name: name });
  };

  const onLogin = async (event) => {
    event.preventDefault();
    const token = await loginUser({ login: login, password: password });
    setToken(token);
  };

  return (
    <div>
        <div className="register-login-wrapper">
            <Button style={{width:'100px'}} onClick={() => setIsLogin(!isLogin)}>{isLogin ? 'Register': 'Login'}</Button>
        </div>
    {isLogin ?
    <div className="login-wrapper">
      <Form onSubmit={onLogin} className="login-form">
        <FormGroup>
            <Label>Login</Label>
            <Input type="text" onChange={(e) => setLogin(e.target.value)} />
        </FormGroup>
        <FormGroup>
          <Label>Password</Label>
          <Input
            type="password"
            onChange={(e) => setPassword(e.target.value)}
          />
        </FormGroup>
        <Button color="primary" type="submit">Login</Button>
      </Form>
    </div> :
    <div className="register-wrapper">
      <Form onSubmit={onRegister} className="register-form">
        <FormGroup>
          <Label>Name</Label>
          <Input type="text" onChange={(e) => setName(e.target.value)} />
        </FormGroup>
        <FormGroup>
          <Label>Login</Label>
          <Input type="text" onChange={(e) => setLogin(e.target.value)} />
        </FormGroup>
        <FormGroup>
          <Label>Password</Label>
          <Input
            type="password"
            onChange={(e) => setPassword(e.target.value)}
          />
        </FormGroup>
        <Button color="primary" type="submit">Register</Button>
      </Form>
    </div>
    }
    </div>
  );
}
