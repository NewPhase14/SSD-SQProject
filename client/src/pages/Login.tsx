import { useState } from "react";
import { authClient } from "../api/clients";
import { useSetAtom } from "jotai";
import { setJwtAtom } from "../state/atoms";

export default function Login({
                                  onNavigate
                              }: {
    onNavigate: (page: string) => void;
}) {
    const setJwt = useSetAtom(setJwtAtom);

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleLogin = async () => {
        setLoading(true);
        setError(null);

        try {
            const res = await authClient.login({ email, password });

            if (res.jwt) {
                setJwt(res.jwt);
                onNavigate("home");
            } else {
                setError("Invalid email or password");
            }
        } catch {
            setError("Something went wrong. Try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-[80vh] flex items-center justify-center bg-gray-100 px-4 text-black">

            <div className="w-full max-w-md bg-white border border-gray-200 shadow-lg rounded-2xl p-8 text-black">

                <div className="text-center mb-6">
                    <h1 className="text-2xl font-bold text-black">
                        Welcome back
                    </h1>
                    <p className="text-sm text-gray-700 mt-1">
                        Sign in to your account to continue
                    </p>
                </div>

                {error && (
                    <div className="mb-4 text-sm text-red-700 bg-red-50 border border-red-200 px-3 py-2 rounded-xl">
                        {error}
                    </div>
                )}

                <input
                    className="w-full mb-3 px-4 py-3 border border-gray-300 rounded-xl bg-white text-black placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Email address"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <input
                    className="w-full mb-5 px-4 py-3 border border-gray-300 rounded-xl bg-white text-black placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />

                <button
                    onClick={handleLogin}
                    disabled={loading}
                    className="w-full bg-blue-600 text-white py-3 rounded-xl font-semibold hover:bg-blue-700 disabled:opacity-60"
                >
                    {loading ? "Signing in..." : "Sign in"}
                </button>

                <p className="text-center text-sm text-gray-700 mt-5">
                    Don’t have an account?{" "}
                    <span
                        onClick={() => onNavigate("register")}
                        className="text-blue-600 hover:underline cursor-pointer"
                    >
                        Sign up
                    </span>
                </p>
            </div>
        </div>
    );
}