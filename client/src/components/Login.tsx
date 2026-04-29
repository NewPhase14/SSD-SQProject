import React, {useState} from 'react';
import {authClient} from '../apiControllerClients'

const Login = () => {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    return (
        <div className="bg-background-black flex items-center justify-center min-h-screen">
            <div className="bg-background-grey border border-white/10 rounded-2xl p-10 max-w-lg">
                <div className="flex flex-col items-center mb-8 w-2/3 mx-auto">
                </div>

                <h2 className="text-center text-white text-xl mb-6">Sign In</h2>

                <div className="space-y-6">
                    <div>
                        <label className="block mb-2 text-sm text-text-grey">Email Address</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            id="email"
                            className="w-full px-4 py-3 rounded-md text-white border border-white/10 bg-textfield-grey focus:outline-white hover:border-white/30"
                        />
                    </div>

                    <div>
                        <label className="block mb-2 text-sm text-text-grey">Password</label>
                        <div>
                            <input
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                id="password"
                                className="w-full px-4 py-3 rounded-md text-white border border-white/10 bg-textfield-grey focus:outline-white hover:border-white/30"
                            />
                        </div>
                    </div>

                    <button
                        onClick={() => authClient.login({email, password}).then(res => console.log(res))}
                        className="w-full py-3 bg-primary-blue text-white rounded-md hover:bg-primary-blue-hover transition-colors"
                     >
                        Sign In
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Login;