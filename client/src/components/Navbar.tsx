import { useAtom } from "jotai";
import { JwtAtom, setJwtAtom } from "../state/atoms";

export default function Navbar({
                                   onNavigate
                               }: {
    onNavigate: (page: string) => void;
}) {
    const [jwt] = useAtom(JwtAtom);
    const [, setJwt] = useAtom(setJwtAtom);

    const handleLogout = () => {
        setJwt(null);
        onNavigate("home");
    };

    return (
        <div className="sticky top-0 z-50 bg-white border-b border-gray-200 text-black">
            <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">

                <div className="flex items-center gap-6">
                    <div className="font-bold text-xl text-blue-600">
                        MarketPlace
                    </div>

                    <button
                        onClick={() => onNavigate("home")}
                        className="text-gray-700 hover:text-black"
                    >
                        Browse
                    </button>

                    {jwt && (
                        <button
                            onClick={() => onNavigate("create")}
                            className="text-gray-700 hover:text-black"
                        >
                            Sell
                        </button>
                    )}
                </div>

                <div>
                    {!jwt ? (
                        <button
                            onClick={() => onNavigate("login")}
                            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                        >
                            Login
                        </button>
                    ) : (
                        <button
                            onClick={handleLogout}
                            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 text-black"
                        >
                            Logout
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
}