import { useState } from "react";
import { authClient } from "../api/clients";
import { useSetAtom } from "jotai";
import { setJwtAtom } from "../state/atoms";

export default function Register({
                                     onNavigate
                                 }: {
    onNavigate: (page: string) => void;
}) {
    const setJwt = useSetAtom(setJwtAtom);

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [name, setName] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleRegister = async () => {
        setLoading(true);
        setError(null);

        try {
            const res = await authClient.register({
                email,
                password,
                name
            });

            if (res.jwt) {
                setJwt(res.jwt);
                onNavigate("home");
            } else {
                setError("Registration failed");
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

                <h1 className="text-2xl font-bold text-black text-center mb-6">
                    Create account
                </h1>

                {error && (
                    <div className="mb-4 text-sm text-red-700 bg-red-50 border border-red-200 p-2 rounded-xl">
                        {error}
                    </div>
                )}

                <input
                    className="w-full mb-3 px-4 py-3 border border-gray-300 rounded-xl text-black bg-white placeholder-gray-500"
                    placeholder="Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />

                <input
                    className="w-full mb-3 px-4 py-3 border border-gray-300 rounded-xl text-black bg-white placeholder-gray-500"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <input
                    className="w-full mb-5 px-4 py-3 border border-gray-300 rounded-xl text-black bg-white placeholder-gray-500"
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />

                <button
                    onClick={handleRegister}
                    disabled={loading}
                    className="w-full bg-blue-600 text-white py-3 rounded-xl font-semibold hover:bg-blue-700 disabled:opacity-60"
                >
                    {loading ? "Creating..." : "Create account"}
                </button>

                <p className="text-center text-sm text-gray-700 mt-4">
                    Already have an account?{" "}
                    <span
                        onClick={() => onNavigate("login")}
                        className="text-blue-600 cursor-pointer hover:underline"
                    >
                        Login
                    </span>
                </p>
            </div>
        </div>
    );
}