import { useState } from "react";
import CategorySelect from "../components/CategorySelect";
import { listingClient } from "../api/clients";
import { useAtomValue } from "jotai";
import { JwtAtom } from "../state/atoms";

export default function CreateListing() {
    const jwt = useAtomValue(JwtAtom);

    const [categoryId, setCategoryId] = useState<string | null>(null);
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [price, setPrice] = useState<number>(0);
    const [condition, setCondition] = useState("");
    const [images, setImages] = useState<File[]>([]);
    const [loading, setLoading] = useState(false);
    const [dragActive, setDragActive] = useState(false);

    if (!jwt) {
        return (
            <div className="max-w-2xl mx-auto mt-20 text-center">
                <div className="text-gray-900 text-lg font-semibold">
                    Login required
                </div>
                <div className="text-gray-500 mt-2">
                    You must be logged in to create a listing.
                </div>
            </div>
        );
    }

    const handleSubmit = async () => {
        if (!categoryId) {
            alert("Please select a category");
            return;
        }

        setLoading(true);

        try {
            const fileParams = images.map((file) => ({
                data: file,
                fileName: file.name,
            }));

            await listingClient.create(
                jwt,
                categoryId,
                condition,
                title,
                description,
                price,
                "Active", // ✅ HARD-CODED STATUS
                fileParams
            );

            alert("Listing created!");

            setTitle("");
            setDescription("");
            setPrice(0);
            setCondition("");
            setImages([]);
            setCategoryId(null);
        } catch (err) {
            console.error(err);
            alert("Failed to create listing");
        } finally {
            setLoading(false);
        }
    };

    const addFiles = (files: FileList | null) => {
        if (!files) return;
        setImages((prev) => [...prev, ...Array.from(files)]);
    };

    return (
        <div className="max-w-3xl mx-auto px-4 py-10">
            <div className="bg-white border border-gray-100 rounded-2xl shadow-sm p-6">

                <h1 className="text-2xl font-bold text-gray-900 mb-6">
                    Create a listing
                </h1>

                <div className="space-y-5">

                    {/* CATEGORY */}
                    <CategorySelect value={categoryId} onChange={setCategoryId} />

                    {/* TITLE */}
                    <input
                        className="w-full border border-gray-200 rounded-xl p-3 text-gray-900 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                        placeholder="Title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />

                    {/* DESCRIPTION */}
                    <textarea
                        className="w-full border border-gray-200 rounded-xl p-3 h-28 text-gray-900 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                        placeholder="Description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                    />

                    {/* PRICE + CONDITION */}
                    <div className="grid grid-cols-2 gap-3">
                        <input
                            className="w-full border border-gray-200 rounded-xl p-3 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            type="number"
                            placeholder="Price (DKK)"
                            value={price}
                            onChange={(e) => setPrice(Number(e.target.value))}
                        />

                        <input
                            className="w-full border border-gray-200 rounded-xl p-3 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="Condition (New / Used)"
                            value={condition}
                            onChange={(e) => setCondition(e.target.value)}
                        />
                    </div>

                    {/* DROPZONE */}
                    <div
                        className={`
                            border-2 border-dashed rounded-xl p-6 text-center cursor-pointer transition
                            ${dragActive ? "border-blue-500 bg-blue-50" : "border-gray-200 bg-white"}
                        `}
                        onDragOver={(e) => {
                            e.preventDefault();
                            setDragActive(true);
                        }}
                        onDragLeave={() => setDragActive(false)}
                        onDrop={(e) => {
                            e.preventDefault();
                            setDragActive(false);
                            addFiles(e.dataTransfer.files);
                        }}
                    >
                        <input
                            id="file-upload"
                            type="file"
                            multiple
                            className="hidden"
                            onChange={(e) => addFiles(e.target.files)}
                        />

                        <label htmlFor="file-upload" className="cursor-pointer">
                            <div className="text-gray-900 font-medium">
                                Drag & drop images here
                            </div>
                            <div className="text-sm text-gray-500 mt-1">
                                or click to browse files
                            </div>
                        </label>
                    </div>

                    {/* IMAGE PREVIEW */}
                    {images.length > 0 && (
                        <div className="grid grid-cols-3 gap-2">
                            {images.map((img, i) => {
                                const url = URL.createObjectURL(img);

                                return (
                                    <div key={i} className="relative">
                                        <img
                                            src={url}
                                            className="h-24 w-full object-cover rounded-lg border"
                                        />
                                        <button
                                            onClick={() =>
                                                setImages((prev) =>
                                                    prev.filter((_, idx) => idx !== i)
                                                )
                                            }
                                            className="absolute top-1 right-1 bg-black/60 text-white text-xs px-2 py-1 rounded"
                                        >
                                            ✕
                                        </button>
                                    </div>
                                );
                            })}
                        </div>
                    )}

                    {/* SUBMIT */}
                    <button
                        onClick={handleSubmit}
                        disabled={loading}
                        className="w-full bg-blue-600 text-white py-3 rounded-xl font-semibold hover:bg-blue-700 transition disabled:opacity-60"
                    >
                        {loading ? "Creating..." : "Create Listing"}
                    </button>

                </div>
            </div>
        </div>
    );
}